import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of } from 'rxjs';
import { LivroService } from '../services/livro.service';
import { LivrosActions } from './livros.actions';

@Injectable()
export class LivrosEffects {
  loadLivros$ = createEffect(() => this.actions$.pipe(
    ofType(LivrosActions.loadLivros),
    mergeMap(() => this.svc.getAll().pipe(
      map(res => LivrosActions.loadLivrosSuccess({ livros: res.data ?? [] })),
      catchError(err => of(LivrosActions.loadLivrosFailure({ error: err.message })))
    ))
  ));
  createLivro$ = createEffect(() => this.actions$.pipe(
    ofType(LivrosActions.createLivro),
    mergeMap(({ dto }) => this.svc.create(dto).pipe(
      map(res => LivrosActions.createLivroSuccess({ livro: res.data! })),
      catchError(err => of(LivrosActions.createLivroFailure({ error: err.message })))
    ))
  ));
  updateLivro$ = createEffect(() => this.actions$.pipe(
    ofType(LivrosActions.updateLivro),
    mergeMap(({ id, dto }) => this.svc.update(id, dto).pipe(
      map(res => LivrosActions.updateLivroSuccess({ livro: res.data! })),
      catchError(err => of(LivrosActions.updateLivroFailure({ error: err.message })))
    ))
  ));
  deleteLivro$ = createEffect(() => this.actions$.pipe(
    ofType(LivrosActions.deleteLivro),
    mergeMap(({ id }) => this.svc.delete(id).pipe(
      map(() => LivrosActions.deleteLivroSuccess({ id })),
      catchError(err => of(LivrosActions.deleteLivroFailure({ error: err.message })))
    ))
  ));
  constructor(private actions$: Actions, private svc: LivroService) {}
}
