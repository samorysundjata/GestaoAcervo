import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Store } from '@ngrx/store';
import { MatDialog } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { AutoresListComponent } from './autores-list.component';
import { AutoresActions } from '../../store/autores.actions';
import { AutorViewModel } from '../../models/autor.model';

describe('AutoresListComponent', () => {
  let fixture: ComponentFixture<AutoresListComponent>;
  let store: jasmine.SpyObj<Store>;
  let dialog: jasmine.SpyObj<MatDialog>;

  const autor: AutorViewModel = { id: '1', nome: 'Pessoa', email: 'p@test.com' };

  beforeEach(async () => {
    store = jasmine.createSpyObj('Store', ['select', 'dispatch']);
    dialog = jasmine.createSpyObj('MatDialog', ['open']);
    const selectResults = [of([autor]), of(false)];
    let selectIndex = 0;
    store.select.and.callFake(() => selectResults[selectIndex++] ?? of(false));

    await TestBed.configureTestingModule({
      imports: [AutoresListComponent, NoopAnimationsModule],
      providers: [
        provideRouter([]),
        { provide: Store, useValue: store },
        { provide: MatDialog, useValue: dialog },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AutoresListComponent);
    fixture.detectChanges();
  });

  it('should dispatch loadAutores on init', () => {
    expect(store.dispatch).toHaveBeenCalledWith(AutoresActions.loadAutores());
  });

  it('should render author name', () => {
    expect(fixture.nativeElement.textContent).toContain('Pessoa');
  });

  it('should dispatch deleteAutor after dialog confirm', () => {
    dialog.open.and.returnValue({ afterClosed: () => of(true) } as never);
    fixture.componentInstance.onDelete('1', 'Pessoa');
    expect(store.dispatch).toHaveBeenCalledWith(AutoresActions.deleteAutor({ id: '1' }));
  });

  it('should not dispatch delete when dialog is cancelled', () => {
    store.dispatch.calls.reset();
    dialog.open.and.returnValue({ afterClosed: () => of(false) } as never);
    fixture.componentInstance.onDelete('1', 'Pessoa');
    expect(store.dispatch).not.toHaveBeenCalled();
  });
});
