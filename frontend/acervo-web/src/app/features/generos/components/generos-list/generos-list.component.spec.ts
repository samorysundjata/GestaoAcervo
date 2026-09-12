import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Store } from '@ngrx/store';
import { MatDialog } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { GenerosListComponent } from './generos-list.component';
import { GenerosActions } from '../../store/generos.actions';
import { GeneroViewModel } from '../../models/genero.model';

describe('GenerosListComponent', () => {
  let fixture: ComponentFixture<GenerosListComponent>;
  let store: jasmine.SpyObj<Store>;
  let dialog: jasmine.SpyObj<MatDialog>;

  const genero: GeneroViewModel = { id: '1', nome: 'Poesia' };

  beforeEach(async () => {
    store = jasmine.createSpyObj('Store', ['select', 'dispatch']);
    dialog = jasmine.createSpyObj('MatDialog', ['open']);
    const selectResults = [of([genero]), of(false)];
    let selectIndex = 0;
    store.select.and.callFake(() => selectResults[selectIndex++] ?? of(false));

    await TestBed.configureTestingModule({
      imports: [GenerosListComponent, NoopAnimationsModule],
      providers: [
        provideRouter([]),
        { provide: Store, useValue: store },
        { provide: MatDialog, useValue: dialog },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(GenerosListComponent);
    fixture.detectChanges();
  });

  it('should dispatch loadGeneros on init', () => {
    expect(store.dispatch).toHaveBeenCalledWith(GenerosActions.loadGeneros());
  });

  it('should render genre name', () => {
    expect(fixture.nativeElement.textContent).toContain('Poesia');
  });

  it('should dispatch deleteGenero after dialog confirm', () => {
    dialog.open.and.returnValue({ afterClosed: () => of(true) } as never);
    fixture.componentInstance.onDelete('1', 'Poesia');
    expect(store.dispatch).toHaveBeenCalledWith(GenerosActions.deleteGenero({ id: '1' }));
  });

  it('should not dispatch delete when dialog is cancelled', () => {
    store.dispatch.calls.reset();
    dialog.open.and.returnValue({ afterClosed: () => of(false) } as never);
    fixture.componentInstance.onDelete('1', 'Poesia');
    expect(store.dispatch).not.toHaveBeenCalled();
  });
});
