import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of } from 'rxjs';
import { AutorService } from '../services/autor.service';
import { AutoresActions } from './autores.actions';

@Injectable()
export class AutoresEffects {
  loadAutores$ = createEffect(() => this.actions$.pipe(
    ofType(AutoresActions.loadAutores),
    mergeMap(() => this.svc.getAll().pipe(
      map(res => AutoresActions.loadAutoresSuccess({ autores: res.data ?? [] })),
      catchError(err => of(AutoresActions.loadAutoresFailure({ error: err.message })))
    ))
  ));

  createAutor$ = createEffect(() => this.actions$.pipe(
    ofType(AutoresActions.createAutor),
    mergeMap(({ dto }) => this.svc.create(dto).pipe(
      map(res => AutoresActions.createAutorSuccess({ autor: res.data! })),
      catchError(err => of(AutoresActions.createAutorFailure({ error: err.message })))
    ))
  ));

  updateAutor$ = createEffect(() => this.actions$.pipe(
    ofType(AutoresActions.updateAutor),
    mergeMap(({ id, dto }) => this.svc.update(id, dto).pipe(
      map(res => AutoresActions.updateAutorSuccess({ autor: res.data! })),
      catchError(err => of(AutoresActions.updateAutorFailure({ error: err.message })))
    ))
  ));

  deleteAutor$ = createEffect(() => this.actions$.pipe(
    ofType(AutoresActions.deleteAutor),
    mergeMap(({ id }) => this.svc.delete(id).pipe(
      map(() => AutoresActions.deleteAutorSuccess({ id })),
      catchError(err => of(AutoresActions.deleteAutorFailure({ error: err.message })))
    ))
  ));

  constructor(private actions$: Actions, private svc: AutorService) {}
}
