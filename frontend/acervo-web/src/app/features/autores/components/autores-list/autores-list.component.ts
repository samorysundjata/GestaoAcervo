import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { AutoresActions } from '../../store/autores.actions';
import { selectAllAutores, selectAutoresLoading } from '../../store/autores.selectors';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-autores-list',
  standalone: true,
  imports: [CommonModule, RouterLink, MatTableModule, MatButtonModule, MatIconModule, LoadingSpinnerComponent],
  templateUrl: './autores-list.component.html'
})
export class AutoresListComponent implements OnInit {
  autores$ = this.store.select(selectAllAutores);
  loading$ = this.store.select(selectAutoresLoading);
  displayedColumns = ['nome', 'email', 'acoes'];

  constructor(private store: Store, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.store.dispatch(AutoresActions.loadAutores());
  }

  onDelete(id: string, nome: string): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Excluir Autor', message: `Deseja excluir o autor "${nome}"?` }
    });
    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) this.store.dispatch(AutoresActions.deleteAutor({ id }));
    });
  }
}
