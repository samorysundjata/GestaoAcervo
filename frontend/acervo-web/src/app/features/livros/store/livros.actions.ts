import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { LivroViewModel } from '../models/livro.model';
import { CreateLivroDto } from '../models/create-livro.dto';
import { UpdateLivroDto } from '../models/update-livro.dto';

export const LivrosActions = createActionGroup({
  source: 'Livros',
  events: {
    'Load Livros': emptyProps(),
    'Load Livros Success': props<{ livros: LivroViewModel[] }>(),
    'Load Livros Failure': props<{ error: string }>(),
    'Create Livro': props<{ dto: CreateLivroDto }>(),
    'Create Livro Success': props<{ livro: LivroViewModel }>(),
    'Create Livro Failure': props<{ error: string }>(),
    'Update Livro': props<{ id: string; dto: UpdateLivroDto }>(),
    'Update Livro Success': props<{ livro: LivroViewModel }>(),
    'Update Livro Failure': props<{ error: string }>(),
    'Delete Livro': props<{ id: string }>(),
    'Delete Livro Success': props<{ id: string }>(),
    'Delete Livro Failure': props<{ error: string }>(),
  }
});
