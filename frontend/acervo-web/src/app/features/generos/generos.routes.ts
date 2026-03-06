import { Routes } from '@angular/router';
import { GenerosListComponent } from './components/generos-list/generos-list.component';
import { GeneroFormComponent } from './components/genero-form/genero-form.component';

export const GENEROS_ROUTES: Routes = [
  { path: '', component: GenerosListComponent },
  { path: 'novo', component: GeneroFormComponent },
  { path: ':id/editar', component: GeneroFormComponent },
];
