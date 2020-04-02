import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpErrorResponse,
  HTTP_INTERCEPTORS,
  HttpRequest,
  HttpHandler,
  HttpEvent
} from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  /*
      Interceptor will intercept http request.
      Interceptor will recognize http error request (in 400 - 500 range).
      We want to catch these kinds of errors.
  */
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse) {
          /* Unauthorized error */
          if (error.status === 401) {
            return throwError(error.statusText);
          }

          /* API global exceptions' error */
          const applicationError = error.headers.get('Application-Error');
          if (applicationError) {
            return throwError(applicationError);
          }

          /* API BadRequest error */
          const serverError = error.error;

          /* API input validation error */
          let modalStateErrors = '';
          if (serverError.errors && typeof serverError.errors === 'object') {
            for (const key in serverError.errors) {
              if (serverError.errors[key]) {
                modalStateErrors += serverError.errors[key] + '\n';
              }
            }
          }
          return throwError(modalStateErrors || serverError || 'Server Error');
        }
      })
    );
  }
}

export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true
};
