import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AutorViewModel } from '../../models/autor.model';

@Component({
  selector: 'app-autor-card',
  standalone: true,
  imports: [MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <mat-card>
      <mat-card-title>{{ autor.nome }}</mat-card-title>
      <mat-card-subtitle>{{ autor.email }}</mat-card-subtitle>
      <mat-card-actions>
        <button mat-icon-button (click)="edit.emit(autor.id)"><mat-icon>edit</mat-icon></button>
        <button mat-icon-button color="warn" (click)="delete.emit(autor.id)"><mat-icon>delete</mat-icon></button>
      </mat-card-actions>
    </mat-card>
  `
})
export class AutorCardComponent {
  @Input() autor!: AutorViewModel;
  @Output() edit = new EventEmitter<string>();
  @Output() delete = new EventEmitter<string>();
}
