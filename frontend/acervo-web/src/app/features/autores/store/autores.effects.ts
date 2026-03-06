import { Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { catchError, map, mergeMap, tap } from "rxjs/operators";
import { of } from "rxjs";
import { AutorService } from "../services/autor.service";
import { AutoresActions } from "./autores.actions";
import { NotificationService } from "../../../core/services/notification.service";

@Injectable()
export class AutoresEffects {
  loadAutores$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AutoresActions.loadAutores),
      mergeMap(() =>
        this.svc.getAll().pipe(
          map((res) =>
            AutoresActions.loadAutoresSuccess({ autores: res.data ?? [] }),
          ),
          catchError((err) =>
            of(AutoresActions.loadAutoresFailure({ error: err.message })),
          ),
        ),
      ),
    ),
  );

  createAutor$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AutoresActions.createAutor),
      mergeMap(({ dto }) =>
        this.svc.create(dto).pipe(
          map((res) => AutoresActions.createAutorSuccess({ autor: res.data! })),
          catchError((err) =>
            of(AutoresActions.createAutorFailure({ error: err.message })),
          ),
        ),
      ),
    ),
  );

  createAutorSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AutoresActions.createAutorSuccess),
        tap(() => this.notification.success("Autor criado com sucesso!")),
      ),
    { dispatch: false },
  );

  updateAutor$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AutoresActions.updateAutor),
      mergeMap(({ id, dto }) =>
        this.svc.update(id, dto).pipe(
          map((res) => AutoresActions.updateAutorSuccess({ autor: res.data! })),
          catchError((err) =>
            of(AutoresActions.updateAutorFailure({ error: err.message })),
          ),
        ),
      ),
    ),
  );

  updateAutorSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AutoresActions.updateAutorSuccess),
        tap(() => this.notification.success("Autor atualizado com sucesso!")),
      ),
    { dispatch: false },
  );

  deleteAutor$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AutoresActions.deleteAutor),
      mergeMap(({ id }) =>
        this.svc.delete(id).pipe(
          map(() => AutoresActions.deleteAutorSuccess({ id })),
          catchError((err) =>
            of(AutoresActions.deleteAutorFailure({ error: err.message })),
          ),
        ),
      ),
    ),
  );

  deleteAutorSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AutoresActions.deleteAutorSuccess),
        tap(() => this.notification.success("Autor excluído com sucesso!")),
      ),
    { dispatch: false },
  );

  constructor(
    private actions$: Actions,
    private svc: AutorService,
    private notification: NotificationService,
  ) {}
}
