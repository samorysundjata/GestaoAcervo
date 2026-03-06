import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'livros', pathMatch: 'full' },
  {
    path: 'autores',
    loadChildren: () => import('./features/autores/autores.routes').then(m => m.AUTORES_ROUTES)
  },
  {
    path: 'generos',
    loadChildren: () => import('./features/generos/generos.routes').then(m => m.GENEROS_ROUTES)
  },
  {
    path: 'livros',
    loadChildren: () => import('./features/livros/livros.routes').then(m => m.LIVROS_ROUTES)
  },
  { path: '**', redirectTo: 'livros' }
];
