import { createFeatureSelector, createSelector } from '@ngrx/store';
import { LivrosState } from './livros.reducer';

export const selectLivrosState = createFeatureSelector<LivrosState>('livros');
export const selectAllLivros = createSelector(selectLivrosState, s => s.livros);
export const selectLivrosLoading = createSelector(selectLivrosState, s => s.loading);
export const selectLivrosError = createSelector(selectLivrosState, s => s.error);
