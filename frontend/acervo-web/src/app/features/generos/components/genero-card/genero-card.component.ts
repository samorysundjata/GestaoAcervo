import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { GeneroViewModel } from '../../models/genero.model';

@Component({
  selector: 'app-genero-card',
  standalone: true,
  imports: [MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <mat-card>
      <mat-card-title>{{ genero.nome }}</mat-card-title>
      <mat-card-actions>
        <button mat-icon-button (click)="edit.emit(genero.id)"><mat-icon>edit</mat-icon></button>
        <button mat-icon-button color="warn" (click)="delete.emit(genero.id)"><mat-icon>delete</mat-icon></button>
      </mat-card-actions>
    </mat-card>
  `
})
export class GeneroCardComponent {
  @Input() genero!: GeneroViewModel;
  @Output() edit = new EventEmitter<string>();
  @Output() delete = new EventEmitter<string>();
}
