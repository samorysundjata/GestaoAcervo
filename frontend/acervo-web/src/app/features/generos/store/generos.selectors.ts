import { createFeatureSelector, createSelector } from '@ngrx/store';
import { GenerosState } from './generos.reducer';

export const selectGenerosState = createFeatureSelector<GenerosState>('generos');
export const selectAllGeneros = createSelector(selectGenerosState, s => s.generos);
export const selectGenerosLoading = createSelector(selectGenerosState, s => s.loading);
