import { autoresReducer, initialState } from './autores.reducer';
import { AutoresActions } from './autores.actions';
import { AutorViewModel } from '../models/autor.model';

describe('autoresReducer', () => {
  const autor: AutorViewModel = { id: '1', nome: 'Pessoa', email: 'p@test.com' };

  it('loadAutores should set loading', () => {
    const state = autoresReducer(initialState, AutoresActions.loadAutores());
    expect(state.loading).toBeTrue();
    expect(state.error).toBeNull();
  });

  it('loadAutoresSuccess should set list', () => {
    const state = autoresReducer(
      { ...initialState, loading: true },
      AutoresActions.loadAutoresSuccess({ autores: [autor] }),
    );
    expect(state.autores).toEqual([autor]);
    expect(state.loading).toBeFalse();
  });

  it('loadAutoresFailure should set error', () => {
    const state = autoresReducer(
      { ...initialState, loading: true },
      AutoresActions.loadAutoresFailure({ error: 'fail' }),
    );
    expect(state.error).toBe('fail');
    expect(state.loading).toBeFalse();
  });

  it('createAutorSuccess should append', () => {
    const state = autoresReducer(initialState, AutoresActions.createAutorSuccess({ autor }));
    expect(state.autores).toEqual([autor]);
  });

  it('updateAutorSuccess should replace matching id', () => {
    const updated = { ...autor, nome: 'Novo' };
    const state = autoresReducer(
      { ...initialState, autores: [autor] },
      AutoresActions.updateAutorSuccess({ autor: updated }),
    );
    expect(state.autores[0].nome).toBe('Novo');
  });

  it('deleteAutorSuccess should remove id', () => {
    const state = autoresReducer(
      { ...initialState, autores: [autor] },
      AutoresActions.deleteAutorSuccess({ id: '1' }),
    );
    expect(state.autores).toEqual([]);
  });
});
