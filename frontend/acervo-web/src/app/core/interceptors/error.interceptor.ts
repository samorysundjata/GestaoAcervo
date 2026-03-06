import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'Ocorreu um erro inesperado.';
      if (error.error?.message) message = error.error.message;
      if (error.error?.errors?.length) message = error.error.errors.join(', ');
      console.error(`[HTTP Error ${error.status}]:`, message);
      return throwError(() => ({ status: error.status, message }));
    })
  );
};
