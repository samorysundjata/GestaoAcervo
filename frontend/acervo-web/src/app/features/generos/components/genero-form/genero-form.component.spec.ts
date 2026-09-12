import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Store } from '@ngrx/store';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { GeneroFormComponent } from './genero-form.component';
import { GeneroService } from '../../services/genero.service';
import { GenerosActions } from '../../store/generos.actions';

describe('GeneroFormComponent', () => {
  let component: GeneroFormComponent;
  let store: jasmine.SpyObj<Store>;
  let svc: jasmine.SpyObj<GeneroService>;

  const ok = <T>(data?: T) => of({ success: true, message: 'ok', data });

  async function setup(id: string | null = null): Promise<void> {
    store = jasmine.createSpyObj('Store', ['dispatch']);
    svc = jasmine.createSpyObj('GeneroService', ['getById']);
    svc.getById.and.returnValue(ok({ id: '1', nome: 'Poesia' }));

    await TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [GeneroFormComponent, NoopAnimationsModule],
      providers: [
        provideRouter([]),
        { provide: Store, useValue: store },
        { provide: GeneroService, useValue: svc },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap(id ? { id } : {}) } } },
      ],
    }).compileComponents();

    const fixture: ComponentFixture<GeneroFormComponent> = TestBed.createComponent(GeneroFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  it('should not dispatch when the form is invalid', async () => {
    await setup();
    component.onSubmit();
    expect(store.dispatch).not.toHaveBeenCalled();
  });

  it('should dispatch createGenero when valid', async () => {
    await setup();
    component.form.setValue({ nome: 'Poesia' });
    component.onSubmit();
    expect(store.dispatch).toHaveBeenCalledWith(GenerosActions.createGenero({ dto: { nome: 'Poesia' } }));
  });

  it('should patch the form and dispatch updateGenero in edit mode', async () => {
    await setup('1');
    expect(component.form.value.nome).toBe('Poesia');
    component.onSubmit();
    expect(store.dispatch).toHaveBeenCalledWith(
      GenerosActions.updateGenero({ id: '1', dto: { nome: 'Poesia' } }),
    );
  });
});
