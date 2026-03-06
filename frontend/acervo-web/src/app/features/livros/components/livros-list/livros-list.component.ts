import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { LivrosActions } from '../../store/livros.actions';
import { selectAllLivros, selectLivrosLoading } from '../../store/livros.selectors';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-livros-list',
  standalone: true,
  imports: [CommonModule, RouterLink, MatTableModule, MatButtonModule, MatIconModule, LoadingSpinnerComponent],
  templateUrl: './livros-list.component.html'
})
export class LivrosListComponent implements OnInit {
  livros$ = this.store.select(selectAllLivros);
  loading$ = this.store.select(selectLivrosLoading);
  displayedColumns = ['titulo', 'isbn', 'anoPublicacao', 'acoes'];

  constructor(private store: Store, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.store.dispatch(LivrosActions.loadLivros());
  }

  onDelete(id: string, titulo: string): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Excluir Livro', message: `Deseja excluir "${titulo}"?` }
    });
    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) this.store.dispatch(LivrosActions.deleteLivro({ id }));
    });
  }
}
