import { selectAllLivros, selectLivrosError, selectLivrosLoading } from './livros.selectors';
import { LivrosState } from './livros.reducer';
import { LivroViewModel } from '../models/livro.model';

describe('livros selectors', () => {
  const livro: LivroViewModel = {
    id: '1',
    titulo: 'Mensagem',
    isbn: '9781234567890',
    anoPublicacao: 1934,
    autorId: 'a1',
    generoId: 'g1',
  };
  const state: LivrosState = { livros: [livro], loading: true, error: 'fail' };

  it('selectAllLivros should return the list', () => {
    expect(selectAllLivros.projector(state)).toEqual([livro]);
  });

  it('selectLivrosLoading should return loading', () => {
    expect(selectLivrosLoading.projector(state)).toBeTrue();
  });

  it('selectLivrosError should return error', () => {
    expect(selectLivrosError.projector(state)).toBe('fail');
  });
});
