import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { AutorViewModel } from '../models/autor.model';
import { CreateAutorDto } from '../models/create-autor.dto';
import { UpdateAutorDto } from '../models/update-autor.dto';

export const AutoresActions = createActionGroup({
  source: 'Autores',
  events: {
    'Load Autores': emptyProps(),
    'Load Autores Success': props<{ autores: AutorViewModel[] }>(),
    'Load Autores Failure': props<{ error: string }>(),
    'Create Autor': props<{ dto: CreateAutorDto }>(),
    'Create Autor Success': props<{ autor: AutorViewModel }>(),
    'Create Autor Failure': props<{ error: string }>(),
    'Update Autor': props<{ id: string; dto: UpdateAutorDto }>(),
    'Update Autor Success': props<{ autor: AutorViewModel }>(),
    'Update Autor Failure': props<{ error: string }>(),
    'Delete Autor': props<{ id: string }>(),
    'Delete Autor Success': props<{ id: string }>(),
    'Delete Autor Failure': props<{ error: string }>(),
  }
});
