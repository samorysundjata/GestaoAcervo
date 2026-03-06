import { createReducer, on } from '@ngrx/store';
import { GeneroViewModel } from '../models/genero.model';
import { GenerosActions } from './generos.actions';

export interface GenerosState {
  generos: GeneroViewModel[];
  loading: boolean;
  error: string | null;
}

export const initialState: GenerosState = { generos: [], loading: false, error: null };

export const generosReducer = createReducer(
  initialState,
  on(GenerosActions.loadGeneros, s => ({ ...s, loading: true, error: null })),
  on(GenerosActions.loadGenerosSuccess, (s, { generos }) => ({ ...s, generos, loading: false })),
  on(GenerosActions.loadGenerosFailure, (s, { error }) => ({ ...s, error, loading: false })),
  on(GenerosActions.createGeneroSuccess, (s, { genero }) => ({ ...s, generos: [...s.generos, genero] })),
  on(GenerosActions.updateGeneroSuccess, (s, { genero }) => ({
    ...s, generos: s.generos.map(g => g.id === genero.id ? genero : g)
  })),
  on(GenerosActions.deleteGeneroSuccess, (s, { id }) => ({
    ...s, generos: s.generos.filter(g => g.id !== id)
  })),
);
