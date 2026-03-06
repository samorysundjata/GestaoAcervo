import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { GeneroViewModel } from '../models/genero.model';
import { CreateGeneroDto } from '../models/create-genero.dto';
import { UpdateGeneroDto } from '../models/update-genero.dto';

export const GenerosActions = createActionGroup({
  source: 'Generos',
  events: {
    'Load Generos': emptyProps(),
    'Load Generos Success': props<{ generos: GeneroViewModel[] }>(),
    'Load Generos Failure': props<{ error: string }>(),
    'Create Genero': props<{ dto: CreateGeneroDto }>(),
    'Create Genero Success': props<{ genero: GeneroViewModel }>(),
    'Create Genero Failure': props<{ error: string }>(),
    'Update Genero': props<{ id: string; dto: UpdateGeneroDto }>(),
    'Update Genero Success': props<{ genero: GeneroViewModel }>(),
    'Update Genero Failure': props<{ error: string }>(),
    'Delete Genero': props<{ id: string }>(),
    'Delete Genero Success': props<{ id: string }>(),
    'Delete Genero Failure': props<{ error: string }>(),
  }
});
