import { Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { catchError, map, mergeMap, tap } from "rxjs/operators";
import { of } from "rxjs";
import { LivroService } from "../services/livro.service";
import { LivrosActions } from "./livros.actions";
import { NotificationService } from "../../../core/services/notification.service";

@Injectable()
export class LivrosEffects {
  loadLivros$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LivrosActions.loadLivros),
      mergeMap(() =>
        this.svc.getAll().pipe(
          map((res) =>
            LivrosActions.loadLivrosSuccess({ livros: res.data ?? [] }),
          ),
          catchError((err) =>
            of(LivrosActions.loadLivrosFailure({ error: err.message })),
          ),
        ),
      ),
    ),
  );

  createLivro$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LivrosActions.createLivro),
      mergeMap(({ dto }) =>
        this.svc.create(dto).pipe(
          map((res) => LivrosActions.createLivroSuccess({ livro: res.data! })),
          catchError((err) =>
            of(LivrosActions.createLivroFailure({ error: err.message })),
          ),
        ),
      ),
    ),
  );

  createLivroSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(LivrosActions.createLivroSuccess),
        tap(() => this.notification.success("Livro criado com sucesso!")),
      ),
    { dispatch: false },
  );

  updateLivro$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LivrosActions.updateLivro),
      mergeMap(({ id, dto }) =>
        this.svc.update(id, dto).pipe(
          map((res) => LivrosActions.updateLivroSuccess({ livro: res.data! })),
          catchError((err) =>
            of(LivrosActions.updateLivroFailure({ error: err.message })),
          ),
        ),
      ),
    ),
  );

  updateLivroSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(LivrosActions.updateLivroSuccess),
        tap(() => this.notification.success("Livro atualizado com sucesso!")),
      ),
    { dispatch: false },
  );

  deleteLivro$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LivrosActions.deleteLivro),
      mergeMap(({ id }) =>
        this.svc.delete(id).pipe(
          map(() => LivrosActions.deleteLivroSuccess({ id })),
          catchError((err) =>
            of(LivrosActions.deleteLivroFailure({ error: err.message })),
          ),
        ),
      ),
    ),
  );

  deleteLivroSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(LivrosActions.deleteLivroSuccess),
        tap(() => this.notification.success("Livro excluído com sucesso!")),
      ),
    { dispatch: false },
  );

  constructor(
    private actions$: Actions,
    private svc: LivroService,
    private notification: NotificationService,
  ) {}
}
