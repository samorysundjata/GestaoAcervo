import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of } from 'rxjs';
import { GeneroService } from '../services/genero.service';
import { GenerosActions } from './generos.actions';

@Injectable()
export class GenerosEffects {
  loadGeneros$ = createEffect(() => this.actions$.pipe(
    ofType(GenerosActions.loadGeneros),
    mergeMap(() => this.svc.getAll().pipe(
      map(res => GenerosActions.loadGenerosSuccess({ generos: res.data ?? [] })),
      catchError(err => of(GenerosActions.loadGenerosFailure({ error: err.message })))
    ))
  ));
  createGenero$ = createEffect(() => this.actions$.pipe(
    ofType(GenerosActions.createGenero),
    mergeMap(({ dto }) => this.svc.create(dto).pipe(
      map(res => GenerosActions.createGeneroSuccess({ genero: res.data! })),
      catchError(err => of(GenerosActions.createGeneroFailure({ error: err.message })))
    ))
  ));
  updateGenero$ = createEffect(() => this.actions$.pipe(
    ofType(GenerosActions.updateGenero),
    mergeMap(({ id, dto }) => this.svc.update(id, dto).pipe(
      map(res => GenerosActions.updateGeneroSuccess({ genero: res.data! })),
      catchError(err => of(GenerosActions.updateGeneroFailure({ error: err.message })))
    ))
  ));
  deleteGenero$ = createEffect(() => this.actions$.pipe(
    ofType(GenerosActions.deleteGenero),
    mergeMap(({ id }) => this.svc.delete(id).pipe(
      map(() => GenerosActions.deleteGeneroSuccess({ id })),
      catchError(err => of(GenerosActions.deleteGeneroFailure({ error: err.message })))
    ))
  ));
  constructor(private actions$: Actions, private svc: GeneroService) {}
}
