import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { GenerosActions } from '../../store/generos.actions';
import { selectAllGeneros, selectGenerosLoading } from '../../store/generos.selectors';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-generos-list',
  standalone: true,
  imports: [CommonModule, RouterLink, MatTableModule, MatButtonModule, MatIconModule, LoadingSpinnerComponent],
  templateUrl: './generos-list.component.html'
})
export class GenerosListComponent implements OnInit {
  generos$ = this.store.select(selectAllGeneros);
  loading$ = this.store.select(selectGenerosLoading);
  displayedColumns = ['nome', 'acoes'];

  constructor(private store: Store, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.store.dispatch(GenerosActions.loadGeneros());
  }

  onDelete(id: string, nome: string): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Excluir Gênero', message: `Deseja excluir o gênero "${nome}"?` }
    });
    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) this.store.dispatch(GenerosActions.deleteGenero({ id }));
    });
  }
}
