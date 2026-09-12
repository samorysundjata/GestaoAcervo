import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { provideMockActions } from '@ngrx/effects/testing';
import { ReplaySubject, firstValueFrom, of, throwError } from 'rxjs';
import { Action } from '@ngrx/store';
import { GenerosEffects } from './generos.effects';
import { GenerosActions } from './generos.actions';
import { GeneroService } from '../services/genero.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GeneroViewModel } from '../models/genero.model';

describe('GenerosEffects', () => {
  let actions$: ReplaySubject<Action>;
  let effects: GenerosEffects;
  let svc: jasmine.SpyObj<GeneroService>;
  let notification: jasmine.SpyObj<NotificationService>;
  let router: jasmine.SpyObj<Router>;

  const genero: GeneroViewModel = { id: '1', nome: 'Poesia' };
  const ok = <T>(data?: T) => of({ success: true, message: 'ok', data });

  beforeEach(() => {
    actions$ = new ReplaySubject<Action>(1);
    svc = jasmine.createSpyObj('GeneroService', ['getAll', 'create', 'update', 'delete']);
    notification = jasmine.createSpyObj('NotificationService', ['success', 'error']);
    router = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        GenerosEffects,
        provideMockActions(() => actions$),
        { provide: GeneroService, useValue: svc },
        { provide: NotificationService, useValue: notification },
        { provide: Router, useValue: router },
      ],
    });
    effects = TestBed.inject(GenerosEffects);
  });

  it('loadGeneros$ should map data', async () => {
    svc.getAll.and.returnValue(ok([genero]));
    const result = firstValueFrom(effects.loadGeneros$);
    actions$.next(GenerosActions.loadGeneros());
    expect(await result).toEqual(GenerosActions.loadGenerosSuccess({ generos: [genero] }));
  });

  it('loadGeneros$ should default missing data to []', async () => {
    svc.getAll.and.returnValue(ok(undefined));
    const result = firstValueFrom(effects.loadGeneros$);
    actions$.next(GenerosActions.loadGeneros());
    expect(await result).toEqual(GenerosActions.loadGenerosSuccess({ generos: [] }));
  });

  it('loadGeneros$ should map failure', async () => {
    svc.getAll.and.returnValue(throwError(() => ({ message: 'fail' })));
    const result = firstValueFrom(effects.loadGeneros$);
    actions$.next(GenerosActions.loadGeneros());
    expect(await result).toEqual(GenerosActions.loadGenerosFailure({ error: 'fail' }));
  });

  it('createGenero$ should map created entity', async () => {
    svc.create.and.returnValue(ok(genero));
    const result = firstValueFrom(effects.createGenero$);
    actions$.next(GenerosActions.createGenero({ dto: { nome: genero.nome } }));
    expect(await result).toEqual(GenerosActions.createGeneroSuccess({ genero }));
  });

  it('createGenero$ should map failure', async () => {
    svc.create.and.returnValue(throwError(() => ({ message: 'dup' })));
    const result = firstValueFrom(effects.createGenero$);
    actions$.next(GenerosActions.createGenero({ dto: { nome: 'x' } }));
    expect(await result).toEqual(GenerosActions.createGeneroFailure({ error: 'dup' }));
  });

  it('createGeneroSuccess$ should notify and navigate', async () => {
    const result = firstValueFrom(effects.createGeneroSuccess$);
    actions$.next(GenerosActions.createGeneroSuccess({ genero }));
    await result;
    expect(notification.success).toHaveBeenCalledWith('Gênero criado com sucesso!');
    expect(router.navigate).toHaveBeenCalledWith(['/generos']);
  });

  it('updateGenero$ should map updated entity', async () => {
    svc.update.and.returnValue(ok(genero));
    const result = firstValueFrom(effects.updateGenero$);
    actions$.next(GenerosActions.updateGenero({ id: '1', dto: { nome: genero.nome } }));
    expect(await result).toEqual(GenerosActions.updateGeneroSuccess({ genero }));
  });

  it('updateGenero$ should map failure', async () => {
    svc.update.and.returnValue(throwError(() => ({ message: 'fail' })));
    const result = firstValueFrom(effects.updateGenero$);
    actions$.next(GenerosActions.updateGenero({ id: '1', dto: { nome: 'x' } }));
    expect(await result).toEqual(GenerosActions.updateGeneroFailure({ error: 'fail' }));
  });

  it('updateGeneroSuccess$ should notify and navigate', async () => {
    const result = firstValueFrom(effects.updateGeneroSuccess$);
    actions$.next(GenerosActions.updateGeneroSuccess({ genero }));
    await result;
    expect(notification.success).toHaveBeenCalledWith('Gênero atualizado com sucesso!');
    expect(router.navigate).toHaveBeenCalledWith(['/generos']);
  });

  it('deleteGenero$ should map id', async () => {
    svc.delete.and.returnValue(of(undefined));
    const result = firstValueFrom(effects.deleteGenero$);
    actions$.next(GenerosActions.deleteGenero({ id: '1' }));
    expect(await result).toEqual(GenerosActions.deleteGeneroSuccess({ id: '1' }));
  });

  it('deleteGenero$ should map failure', async () => {
    svc.delete.and.returnValue(throwError(() => ({ message: 'vinculados' })));
    const result = firstValueFrom(effects.deleteGenero$);
    actions$.next(GenerosActions.deleteGenero({ id: '1' }));
    expect(await result).toEqual(GenerosActions.deleteGeneroFailure({ error: 'vinculados' }));
  });

  it('deleteGeneroSuccess$ should notify without navigating', async () => {
    const result = firstValueFrom(effects.deleteGeneroSuccess$);
    actions$.next(GenerosActions.deleteGeneroSuccess({ id: '1' }));
    await result;
    expect(notification.success).toHaveBeenCalledWith('Gênero excluído com sucesso!');
    expect(router.navigate).not.toHaveBeenCalled();
  });
});
