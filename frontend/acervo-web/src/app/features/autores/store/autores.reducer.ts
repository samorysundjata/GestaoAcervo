import { createReducer, on } from '@ngrx/store';
import { AutorViewModel } from '../models/autor.model';
import { AutoresActions } from './autores.actions';

export interface AutoresState {
  autores: AutorViewModel[];
  loading: boolean;
  error: string | null;
}

export const initialState: AutoresState = { autores: [], loading: false, error: null };

export const autoresReducer = createReducer(
  initialState,
  on(AutoresActions.loadAutores, s => ({ ...s, loading: true, error: null })),
  on(AutoresActions.loadAutoresSuccess, (s, { autores }) => ({ ...s, autores, loading: false })),
  on(AutoresActions.loadAutoresFailure, (s, { error }) => ({ ...s, error, loading: false })),
  on(AutoresActions.createAutorSuccess, (s, { autor }) => ({ ...s, autores: [...s.autores, autor] })),
  on(AutoresActions.updateAutorSuccess, (s, { autor }) => ({
    ...s, autores: s.autores.map(a => a.id === autor.id ? autor : a)
  })),
  on(AutoresActions.deleteAutorSuccess, (s, { id }) => ({
    ...s, autores: s.autores.filter(a => a.id !== id)
  })),
);
