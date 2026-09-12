import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { provideMockActions } from '@ngrx/effects/testing';
import { ReplaySubject, firstValueFrom, of, throwError } from 'rxjs';
import { Action } from '@ngrx/store';
import { AutoresEffects } from './autores.effects';
import { AutoresActions } from './autores.actions';
import { AutorService } from '../services/autor.service';
import { NotificationService } from '../../../core/services/notification.service';
import { AutorViewModel } from '../models/autor.model';

describe('AutoresEffects', () => {
  let actions$: ReplaySubject<Action>;
  let effects: AutoresEffects;
  let svc: jasmine.SpyObj<AutorService>;
  let notification: jasmine.SpyObj<NotificationService>;
  let router: jasmine.SpyObj<Router>;

  const autor: AutorViewModel = { id: '1', nome: 'Pessoa', email: 'p@test.com' };
  const ok = <T>(data?: T) => of({ success: true, message: 'ok', data });

  beforeEach(() => {
    actions$ = new ReplaySubject<Action>(1);
    svc = jasmine.createSpyObj('AutorService', ['getAll', 'create', 'update', 'delete']);
    notification = jasmine.createSpyObj('NotificationService', ['success', 'error']);
    router = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AutoresEffects,
        provideMockActions(() => actions$),
        { provide: AutorService, useValue: svc },
        { provide: NotificationService, useValue: notification },
        { provide: Router, useValue: router },
      ],
    });
    effects = TestBed.inject(AutoresEffects);
  });

  it('loadAutores$ should map data', async () => {
    svc.getAll.and.returnValue(ok([autor]));
    const result = firstValueFrom(effects.loadAutores$);
    actions$.next(AutoresActions.loadAutores());
    expect(await result).toEqual(AutoresActions.loadAutoresSuccess({ autores: [autor] }));
  });

  it('loadAutores$ should default missing data to []', async () => {
    svc.getAll.and.returnValue(ok(undefined));
    const result = firstValueFrom(effects.loadAutores$);
    actions$.next(AutoresActions.loadAutores());
    expect(await result).toEqual(AutoresActions.loadAutoresSuccess({ autores: [] }));
  });

  it('loadAutores$ should map failure', async () => {
    svc.getAll.and.returnValue(throwError(() => ({ message: 'fail' })));
    const result = firstValueFrom(effects.loadAutores$);
    actions$.next(AutoresActions.loadAutores());
    expect(await result).toEqual(AutoresActions.loadAutoresFailure({ error: 'fail' }));
  });

  it('createAutor$ should map created entity', async () => {
    svc.create.and.returnValue(ok(autor));
    const result = firstValueFrom(effects.createAutor$);
    actions$.next(AutoresActions.createAutor({ dto: { nome: autor.nome, email: autor.email } }));
    expect(await result).toEqual(AutoresActions.createAutorSuccess({ autor }));
  });

  it('createAutor$ should map failure', async () => {
    svc.create.and.returnValue(throwError(() => ({ message: 'dup' })));
    const result = firstValueFrom(effects.createAutor$);
    actions$.next(AutoresActions.createAutor({ dto: { nome: 'x', email: 'x@test.com' } }));
    expect(await result).toEqual(AutoresActions.createAutorFailure({ error: 'dup' }));
  });

  it('createAutorSuccess$ should notify and navigate', async () => {
    const result = firstValueFrom(effects.createAutorSuccess$);
    actions$.next(AutoresActions.createAutorSuccess({ autor }));
    await result;
    expect(notification.success).toHaveBeenCalledWith('Autor criado com sucesso!');
    expect(router.navigate).toHaveBeenCalledWith(['/autores']);
  });

  it('updateAutor$ should map updated entity', async () => {
    svc.update.and.returnValue(ok(autor));
    const result = firstValueFrom(effects.updateAutor$);
    actions$.next(AutoresActions.updateAutor({ id: '1', dto: { nome: autor.nome, email: autor.email } }));
    expect(await result).toEqual(AutoresActions.updateAutorSuccess({ autor }));
  });

  it('updateAutor$ should map failure', async () => {
    svc.update.and.returnValue(throwError(() => ({ message: 'fail' })));
    const result = firstValueFrom(effects.updateAutor$);
    actions$.next(AutoresActions.updateAutor({ id: '1', dto: { nome: 'x', email: 'x@test.com' } }));
    expect(await result).toEqual(AutoresActions.updateAutorFailure({ error: 'fail' }));
  });

  it('updateAutorSuccess$ should notify and navigate', async () => {
    const result = firstValueFrom(effects.updateAutorSuccess$);
    actions$.next(AutoresActions.updateAutorSuccess({ autor }));
    await result;
    expect(notification.success).toHaveBeenCalledWith('Autor atualizado com sucesso!');
    expect(router.navigate).toHaveBeenCalledWith(['/autores']);
  });

  it('deleteAutor$ should map id', async () => {
    svc.delete.and.returnValue(of(undefined));
    const result = firstValueFrom(effects.deleteAutor$);
    actions$.next(AutoresActions.deleteAutor({ id: '1' }));
    expect(await result).toEqual(AutoresActions.deleteAutorSuccess({ id: '1' }));
  });

  it('deleteAutor$ should map failure', async () => {
    svc.delete.and.returnValue(throwError(() => ({ message: 'vinculados' })));
    const result = firstValueFrom(effects.deleteAutor$);
    actions$.next(AutoresActions.deleteAutor({ id: '1' }));
    expect(await result).toEqual(AutoresActions.deleteAutorFailure({ error: 'vinculados' }));
  });

  it('deleteAutorSuccess$ should notify without navigating', async () => {
    const result = firstValueFrom(effects.deleteAutorSuccess$);
    actions$.next(AutoresActions.deleteAutorSuccess({ id: '1' }));
    await result;
    expect(notification.success).toHaveBeenCalledWith('Autor excluído com sucesso!');
    expect(router.navigate).not.toHaveBeenCalled();
  });
});
