import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Store } from '@ngrx/store';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { AutorFormComponent } from './autor-form.component';
import { AutorService } from '../../services/autor.service';
import { AutoresActions } from '../../store/autores.actions';

describe('AutorFormComponent', () => {
  let component: AutorFormComponent;
  let store: jasmine.SpyObj<Store>;
  let svc: jasmine.SpyObj<AutorService>;

  const ok = <T>(data?: T) => of({ success: true, message: 'ok', data });

  async function setup(id: string | null = null): Promise<void> {
    store = jasmine.createSpyObj('Store', ['dispatch']);
    svc = jasmine.createSpyObj('AutorService', ['getById']);
    svc.getById.and.returnValue(ok({ id: '1', nome: 'Pessoa', email: 'p@test.com' }));

    await TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [AutorFormComponent, NoopAnimationsModule],
      providers: [
        provideRouter([]),
        { provide: Store, useValue: store },
        { provide: AutorService, useValue: svc },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap(id ? { id } : {}) } } },
      ],
    }).compileComponents();

    const fixture: ComponentFixture<AutorFormComponent> = TestBed.createComponent(AutorFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  it('should not dispatch when the form is invalid', async () => {
    await setup();
    component.onSubmit();
    expect(store.dispatch).not.toHaveBeenCalled();
  });

  it('should dispatch createAutor when valid', async () => {
    await setup();
    component.form.setValue({ nome: 'Pessoa', email: 'p@test.com' });
    component.onSubmit();
    expect(store.dispatch).toHaveBeenCalledWith(
      AutoresActions.createAutor({ dto: { nome: 'Pessoa', email: 'p@test.com' } }),
    );
  });

  it('should patch the form and dispatch updateAutor in edit mode', async () => {
    await setup('1');
    expect(component.form.value.nome).toBe('Pessoa');
    component.onSubmit();
    expect(store.dispatch).toHaveBeenCalledWith(
      AutoresActions.updateAutor({ id: '1', dto: { nome: 'Pessoa', email: 'p@test.com' } }),
    );
  });
});
