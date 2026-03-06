import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AutoresState } from './autores.reducer';

export const selectAutoresState = createFeatureSelector<AutoresState>('autores');
export const selectAllAutores = createSelector(selectAutoresState, s => s.autores);
export const selectAutoresLoading = createSelector(selectAutoresState, s => s.loading);
export const selectAutoresError = createSelector(selectAutoresState, s => s.error);
