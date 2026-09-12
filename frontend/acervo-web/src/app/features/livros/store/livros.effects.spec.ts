import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { provideMockActions } from '@ngrx/effects/testing';
import { ReplaySubject, firstValueFrom, of, throwError } from 'rxjs';
import { Action } from '@ngrx/store';
import { LivrosEffects } from './livros.effects';
import { LivrosActions } from './livros.actions';
import { LivroService } from '../services/livro.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LivroViewModel } from '../models/livro.model';

describe('LivrosEffects', () => {
  let actions$: ReplaySubject<Action>;
  let effects: LivrosEffects;
  let svc: jasmine.SpyObj<LivroService>;
  let notification: jasmine.SpyObj<NotificationService>;
  let router: jasmine.SpyObj<Router>;

  const livro: LivroViewModel = {
    id: '1',
    titulo: 'Mensagem',
    isbn: '9781234567890',
    anoPublicacao: 1934,
    autorId: 'a1',
    generoId: 'g1',
  };

  const ok = <T>(data?: T) => of({ success: true, message: 'ok', data });

  beforeEach(() => {
    actions$ = new ReplaySubject<Action>(1);
    svc = jasmine.createSpyObj('LivroService', ['getAll', 'create', 'update', 'delete']);
    notification = jasmine.createSpyObj('NotificationService', ['success', 'error']);
    router = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        LivrosEffects,
        provideMockActions(() => actions$),
        { provide: LivroService, useValue: svc },
        { provide: NotificationService, useValue: notification },
        { provide: Router, useValue: router },
      ],
    });
    effects = TestBed.inject(LivrosEffects);
  });

  it('loadLivros$ should map data', async () => {
    svc.getAll.and.returnValue(ok([livro]));
    const result = firstValueFrom(effects.loadLivros$);
    actions$.next(LivrosActions.loadLivros());
    expect(await result).toEqual(LivrosActions.loadLivrosSuccess({ livros: [livro] }));
  });

  it('loadLivros$ should default missing data to []', async () => {
    svc.getAll.and.returnValue(ok(undefined));
    const result = firstValueFrom(effects.loadLivros$);
    actions$.next(LivrosActions.loadLivros());
    expect(await result).toEqual(LivrosActions.loadLivrosSuccess({ livros: [] }));
  });

  it('loadLivros$ should map failure', async () => {
    svc.getAll.and.returnValue(throwError(() => ({ message: 'fail' })));
    const result = firstValueFrom(effects.loadLivros$);
    actions$.next(LivrosActions.loadLivros());
    expect(await result).toEqual(LivrosActions.loadLivrosFailure({ error: 'fail' }));
  });

  it('createLivro$ should map created entity', async () => {
    const dto = { titulo: livro.titulo, isbn: livro.isbn, anoPublicacao: livro.anoPublicacao, autorId: livro.autorId, generoId: livro.generoId };
    svc.create.and.returnValue(ok(livro));
    const result = firstValueFrom(effects.createLivro$);
    actions$.next(LivrosActions.createLivro({ dto }));
    expect(await result).toEqual(LivrosActions.createLivroSuccess({ livro }));
  });

  it('createLivro$ should map failure', async () => {
    svc.create.and.returnValue(throwError(() => ({ message: 'dup' })));
    const result = firstValueFrom(effects.createLivro$);
    actions$.next(LivrosActions.createLivro({
      dto: { titulo: 'x', isbn: '9781234567', anoPublicacao: 1934, autorId: 'a', generoId: 'g' },
    }));
    expect(await result).toEqual(LivrosActions.createLivroFailure({ error: 'dup' }));
  });

  it('createLivroSuccess$ should notify and navigate', async () => {
    const result = firstValueFrom(effects.createLivroSuccess$);
    actions$.next(LivrosActions.createLivroSuccess({ livro }));
    await result;
    expect(notification.success).toHaveBeenCalledWith('Livro criado com sucesso!');
    expect(router.navigate).toHaveBeenCalledWith(['/livros']);
  });

  it('updateLivro$ should map updated entity', async () => {
    svc.update.and.returnValue(ok(livro));
    const result = firstValueFrom(effects.updateLivro$);
    actions$.next(LivrosActions.updateLivro({ id: '1', dto: { titulo: livro.titulo, isbn: livro.isbn, anoPublicacao: livro.anoPublicacao, autorId: livro.autorId, generoId: livro.generoId } }));
    expect(await result).toEqual(LivrosActions.updateLivroSuccess({ livro }));
  });

  it('updateLivro$ should map failure', async () => {
    svc.update.and.returnValue(throwError(() => ({ message: 'fail' })));
    const result = firstValueFrom(effects.updateLivro$);
    actions$.next(LivrosActions.updateLivro({
      id: '1',
      dto: { titulo: 'x', isbn: '9781234567', anoPublicacao: 1934, autorId: 'a', generoId: 'g' },
    }));
    expect(await result).toEqual(LivrosActions.updateLivroFailure({ error: 'fail' }));
  });

  it('updateLivroSuccess$ should notify and navigate', async () => {
    const result = firstValueFrom(effects.updateLivroSuccess$);
    actions$.next(LivrosActions.updateLivroSuccess({ livro }));
    await result;
    expect(notification.success).toHaveBeenCalledWith('Livro atualizado com sucesso!');
    expect(router.navigate).toHaveBeenCalledWith(['/livros']);
  });

  it('deleteLivro$ should map id', async () => {
    svc.delete.and.returnValue(of(undefined));
    const result = firstValueFrom(effects.deleteLivro$);
    actions$.next(LivrosActions.deleteLivro({ id: '1' }));
    expect(await result).toEqual(LivrosActions.deleteLivroSuccess({ id: '1' }));
  });

  it('deleteLivro$ should map failure', async () => {
    svc.delete.and.returnValue(throwError(() => ({ message: 'fail' })));
    const result = firstValueFrom(effects.deleteLivro$);
    actions$.next(LivrosActions.deleteLivro({ id: '1' }));
    expect(await result).toEqual(LivrosActions.deleteLivroFailure({ error: 'fail' }));
  });

  it('deleteLivroSuccess$ should notify without navigating', async () => {
    const result = firstValueFrom(effects.deleteLivroSuccess$);
    actions$.next(LivrosActions.deleteLivroSuccess({ id: '1' }));
    await result;
    expect(notification.success).toHaveBeenCalledWith('Livro excluído com sucesso!');
    expect(router.navigate).not.toHaveBeenCalled();
  });
});
