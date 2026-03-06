import { createReducer, on } from '@ngrx/store';
import { LivroViewModel } from '../models/livro.model';
import { LivrosActions } from './livros.actions';

export interface LivrosState {
  livros: LivroViewModel[];
  loading: boolean;
  error: string | null;
}

export const initialState: LivrosState = { livros: [], loading: false, error: null };

export const livrosReducer = createReducer(
  initialState,
  on(LivrosActions.loadLivros, s => ({ ...s, loading: true, error: null })),
  on(LivrosActions.loadLivrosSuccess, (s, { livros }) => ({ ...s, livros, loading: false })),
  on(LivrosActions.loadLivrosFailure, (s, { error }) => ({ ...s, error, loading: false })),
  on(LivrosActions.createLivroSuccess, (s, { livro }) => ({ ...s, livros: [...s.livros, livro] })),
  on(LivrosActions.updateLivroSuccess, (s, { livro }) => ({
    ...s, livros: s.livros.map(l => l.id === livro.id ? livro : l)
  })),
  on(LivrosActions.deleteLivroSuccess, (s, { id }) => ({
    ...s, livros: s.livros.filter(l => l.id !== id)
  })),
);
