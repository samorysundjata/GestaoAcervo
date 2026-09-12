import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Store } from '@ngrx/store';
import { MatDialog } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { LivrosListComponent } from './livros-list.component';
import { LivrosActions } from '../../store/livros.actions';
import { LivroViewModel } from '../../models/livro.model';

describe('LivrosListComponent', () => {
  let fixture: ComponentFixture<LivrosListComponent>;
  let store: jasmine.SpyObj<Store>;
  let dialog: jasmine.SpyObj<MatDialog>;

  const livro: LivroViewModel = {
    id: '1',
    titulo: 'Mensagem',
    isbn: '9781234567890',
    anoPublicacao: 1934,
    autorId: 'a1',
    generoId: 'g1',
  };

  beforeEach(async () => {
    store = jasmine.createSpyObj('Store', ['select', 'dispatch']);
    dialog = jasmine.createSpyObj('MatDialog', ['open']);
    const selectResults = [of([livro]), of(false)];
    let selectIndex = 0;
    store.select.and.callFake(() => selectResults[selectIndex++] ?? of(false));

    await TestBed.configureTestingModule({
      imports: [LivrosListComponent, NoopAnimationsModule],
      providers: [
        provideRouter([]),
        { provide: Store, useValue: store },
        { provide: MatDialog, useValue: dialog },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LivrosListComponent);
    fixture.detectChanges();
  });

  it('should dispatch loadLivros on init', () => {
    expect(store.dispatch).toHaveBeenCalledWith(LivrosActions.loadLivros());
  });

  it('should render book title', () => {
    expect(fixture.nativeElement.textContent).toContain('Mensagem');
  });

  it('should dispatch deleteLivro after dialog confirm', () => {
    dialog.open.and.returnValue({ afterClosed: () => of(true) } as never);
    fixture.componentInstance.onDelete('1', 'Mensagem');
    expect(dialog.open).toHaveBeenCalled();
    expect(store.dispatch).toHaveBeenCalledWith(LivrosActions.deleteLivro({ id: '1' }));
  });

  it('should not dispatch delete when dialog is cancelled', () => {
    store.dispatch.calls.reset();
    dialog.open.and.returnValue({ afterClosed: () => of(false) } as never);
    fixture.componentInstance.onDelete('1', 'Mensagem');
    expect(store.dispatch).not.toHaveBeenCalled();
  });
});
