import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { LivroViewModel } from '../../models/livro.model';

@Component({
  selector: 'app-livro-card',
  standalone: true,
  imports: [MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <mat-card>
      <mat-card-title>{{ livro.titulo }}</mat-card-title>
      <mat-card-subtitle>ISBN: {{ livro.isbn }} | Ano: {{ livro.anoPublicacao }}</mat-card-subtitle>
      <mat-card-actions>
        <button mat-icon-button (click)="edit.emit(livro.id)"><mat-icon>edit</mat-icon></button>
        <button mat-icon-button color="warn" (click)="delete.emit(livro.id)"><mat-icon>delete</mat-icon></button>
      </mat-card-actions>
    </mat-card>
  `
})
export class LivroCardComponent {
  @Input() livro!: LivroViewModel;
  @Output() edit = new EventEmitter<string>();
  @Output() delete = new EventEmitter<string>();
}
