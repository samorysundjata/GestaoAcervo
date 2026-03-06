import { HttpInterceptorFn, HttpErrorResponse } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, throwError } from "rxjs";
import { NotificationService } from "../services/notification.service";

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = "Ocorreu um erro inesperado.";

      if (error.error?.errors?.length) {
        message = error.error.errors.join(" | ");
      } else if (error.error?.message) {
        message = error.error.message;
      }

      notification.error(message);
      return throwError(() => ({ status: error.status, message }));
    }),
  );
};
