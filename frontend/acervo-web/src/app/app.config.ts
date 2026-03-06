import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { routes } from './app.routes';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { autoresReducer } from './features/autores/store/autores.reducer';
import { AutoresEffects } from './features/autores/store/autores.effects';
import { generosReducer } from './features/generos/store/generos.reducer';
import { GenerosEffects } from './features/generos/store/generos.effects';
import { livrosReducer } from './features/livros/store/livros.reducer';
import { LivrosEffects } from './features/livros/store/livros.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([errorInterceptor])),
    provideAnimations(),
    provideStore({
      autores: autoresReducer,
      generos: generosReducer,
      livros: livrosReducer,
    }),
    provideEffects([AutoresEffects, GenerosEffects, LivrosEffects]),
    provideStoreDevtools({ maxAge: 25 }),
  ]
};
