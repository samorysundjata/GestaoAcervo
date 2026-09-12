import { generosReducer, initialState } from './generos.reducer';
import { GenerosActions } from './generos.actions';
import { GeneroViewModel } from '../models/genero.model';

describe('generosReducer', () => {
  const genero: GeneroViewModel = { id: '1', nome: 'Poesia' };

  it('loadGeneros should set loading', () => {
    const state = generosReducer(initialState, GenerosActions.loadGeneros());
    expect(state.loading).toBeTrue();
    expect(state.error).toBeNull();
  });

  it('loadGenerosSuccess should set list', () => {
    const state = generosReducer(
      { ...initialState, loading: true },
      GenerosActions.loadGenerosSuccess({ generos: [genero] }),
    );
    expect(state.generos).toEqual([genero]);
    expect(state.loading).toBeFalse();
  });

  it('loadGenerosFailure should set error', () => {
    const state = generosReducer(
      { ...initialState, loading: true },
      GenerosActions.loadGenerosFailure({ error: 'fail' }),
    );
    expect(state.error).toBe('fail');
    expect(state.loading).toBeFalse();
  });

  it('createGeneroSuccess should append', () => {
    const state = generosReducer(initialState, GenerosActions.createGeneroSuccess({ genero }));
    expect(state.generos).toEqual([genero]);
  });

  it('updateGeneroSuccess should replace matching id', () => {
    const updated = { ...genero, nome: 'Romance' };
    const state = generosReducer(
      { ...initialState, generos: [genero] },
      GenerosActions.updateGeneroSuccess({ genero: updated }),
    );
    expect(state.generos[0].nome).toBe('Romance');
  });

  it('deleteGeneroSuccess should remove id', () => {
    const state = generosReducer(
      { ...initialState, generos: [genero] },
      GenerosActions.deleteGeneroSuccess({ id: '1' }),
    );
    expect(state.generos).toEqual([]);
  });
});
