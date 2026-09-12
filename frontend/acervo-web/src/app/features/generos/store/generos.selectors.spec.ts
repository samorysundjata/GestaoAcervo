import { selectAllGeneros, selectGenerosLoading } from './generos.selectors';
import { GenerosState } from './generos.reducer';

describe('generos selectors', () => {
  const state: GenerosState = {
    generos: [{ id: '1', nome: 'Poesia' }],
    loading: false,
    error: null,
  };

  it('selectAllGeneros should return the list', () => {
    expect(selectAllGeneros.projector(state)).toEqual(state.generos);
  });

  it('selectGenerosLoading should return loading', () => {
    expect(selectGenerosLoading.projector(state)).toBeFalse();
  });
});
