import { Routes } from '@angular/router';
import { AutoresListComponent } from './components/autores-list/autores-list.component';
import { AutorFormComponent } from './components/autor-form/autor-form.component';

export const AUTORES_ROUTES: Routes = [
  { path: '', component: AutoresListComponent },
  { path: 'novo', component: AutorFormComponent },
  { path: ':id/editar', component: AutorFormComponent },
];
