import { selectAllAutores, selectAutoresError, selectAutoresLoading } from './autores.selectors';
import { AutoresState } from './autores.reducer';

describe('autores selectors', () => {
  const state: AutoresState = {
    autores: [{ id: '1', nome: 'Pessoa', email: 'p@test.com' }],
    loading: true,
    error: 'fail',
  };

  it('selectAllAutores should return the list', () => {
    expect(selectAllAutores.projector(state)).toEqual(state.autores);
  });

  it('selectAutoresLoading should return loading', () => {
    expect(selectAutoresLoading.projector(state)).toBeTrue();
  });

  it('selectAutoresError should return error', () => {
    expect(selectAutoresError.projector(state)).toBe('fail');
  });
});
