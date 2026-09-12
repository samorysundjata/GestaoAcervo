import { livrosReducer, initialState } from './livros.reducer';
import { LivrosActions } from './livros.actions';
import { LivroViewModel } from '../models/livro.model';

describe('livrosReducer', () => {
  const livro: LivroViewModel = {
    id: '1',
    titulo: 'Mensagem',
    isbn: '9781234567890',
    anoPublicacao: 1934,
    autorId: 'a1',
    generoId: 'g1',
  };

  it('loadLivros should set loading', () => {
    const state = livrosReducer(initialState, LivrosActions.loadLivros());
    expect(state.loading).toBeTrue();
    expect(state.error).toBeNull();
  });

  it('loadLivrosSuccess should set list', () => {
    const state = livrosReducer(
      { ...initialState, loading: true },
      LivrosActions.loadLivrosSuccess({ livros: [livro] }),
    );
    expect(state.livros).toEqual([livro]);
    expect(state.loading).toBeFalse();
  });

  it('loadLivrosFailure should set error', () => {
    const state = livrosReducer(
      { ...initialState, loading: true },
      LivrosActions.loadLivrosFailure({ error: 'fail' }),
    );
    expect(state.error).toBe('fail');
    expect(state.loading).toBeFalse();
  });

  it('createLivroSuccess should append', () => {
    const state = livrosReducer(initialState, LivrosActions.createLivroSuccess({ livro }));
    expect(state.livros).toEqual([livro]);
  });

  it('updateLivroSuccess should replace matching id', () => {
    const updated = { ...livro, titulo: 'Orpheu' };
    const state = livrosReducer(
      { ...initialState, livros: [livro] },
      LivrosActions.updateLivroSuccess({ livro: updated }),
    );
    expect(state.livros[0].titulo).toBe('Orpheu');
  });

  it('deleteLivroSuccess should remove id', () => {
    const state = livrosReducer(
      { ...initialState, livros: [livro] },
      LivrosActions.deleteLivroSuccess({ id: '1' }),
    );
    expect(state.livros).toEqual([]);
  });
});
