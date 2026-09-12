import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Store } from '@ngrx/store';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { LivroFormComponent } from './livro-form.component';
import { LivroService } from '../../services/livro.service';
import { AutorService } from '../../../autores/services/autor.service';
import { GeneroService } from '../../../generos/services/genero.service';
import { LivrosActions } from '../../store/livros.actions';

describe('LivroFormComponent', () => {
  let fixture: ComponentFixture<LivroFormComponent>;
  let component: LivroFormComponent;
  let store: jasmine.SpyObj<Store>;
  let livroSvc: jasmine.SpyObj<LivroService>;

  const ok = <T>(data?: T) => of({ success: true, message: 'ok', data });

  async function setup(id: string | null = null): Promise<void> {
    store = jasmine.createSpyObj('Store', ['dispatch']);
    livroSvc = jasmine.createSpyObj('LivroService', ['getById']);
    const autorSvc = jasmine.createSpyObj('AutorService', ['getAll']);
    const generoSvc = jasmine.createSpyObj('GeneroService', ['getAll']);
    autorSvc.getAll.and.returnValue(ok([{ id: 'a1', nome: 'Pessoa', email: 'p@test.com' }]));
    generoSvc.getAll.and.returnValue(ok([{ id: 'g1', nome: 'Poesia' }]));
    livroSvc.getById.and.returnValue(ok({
      id: '1',
      titulo: 'Mensagem',
      isbn: '9781234567890',
      anoPublicacao: 1934,
      autorId: 'a1',
      autorNome: 'Pessoa',
      generoId: 'g1',
      generoNome: 'Poesia',
    }));

    await TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [LivroFormComponent, NoopAnimationsModule],
      providers: [
        provideRouter([]),
        { provide: Store, useValue: store },
        { provide: LivroService, useValue: livroSvc },
        { provide: AutorService, useValue: autorSvc },
        { provide: GeneroService, useValue: generoSvc },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap(id ? { id } : {}) } } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LivroFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  it('should not dispatch when the form is invalid', async () => {
    await setup();
    component.onSubmit();
    expect(store.dispatch).not.toHaveBeenCalled();
  });

  it('should dispatch createLivro when valid', async () => {
    await setup();
    component.form.setValue({
      titulo: 'Mensagem',
      isbn: '9781234567890',
      anoPublicacao: 1934,
      autorId: 'a1',
      generoId: 'g1',
    });
    component.onSubmit();
    expect(store.dispatch).toHaveBeenCalledWith(LivrosActions.createLivro({
      dto: {
        titulo: 'Mensagem',
        isbn: '9781234567890',
        anoPublicacao: 1934,
        autorId: 'a1',
        generoId: 'g1',
      },
    }));
  });

  it('should patch the form and dispatch updateLivro in edit mode', async () => {
    await setup('1');
    expect(component.editId).toBe('1');
    expect(component.form.value.titulo).toBe('Mensagem');
    component.onSubmit();
    expect(store.dispatch).toHaveBeenCalledWith(jasmine.objectContaining({
      type: LivrosActions.updateLivro.type,
    }));
  });

  it('should load empty dropdowns when lookups have no data', async () => {
    store = jasmine.createSpyObj('Store', ['dispatch']);
    livroSvc = jasmine.createSpyObj('LivroService', ['getById']);
    const autorSvc = jasmine.createSpyObj('AutorService', ['getAll']);
    const generoSvc = jasmine.createSpyObj('GeneroService', ['getAll']);
    autorSvc.getAll.and.returnValue(ok(undefined));
    generoSvc.getAll.and.returnValue(ok(undefined));

    await TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [LivroFormComponent, NoopAnimationsModule],
      providers: [
        provideRouter([]),
        { provide: Store, useValue: store },
        { provide: LivroService, useValue: livroSvc },
        { provide: AutorService, useValue: autorSvc },
        { provide: GeneroService, useValue: generoSvc },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap({}) } } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LivroFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    expect(component.autores).toEqual([]);
    expect(component.generos).toEqual([]);
  });
});
