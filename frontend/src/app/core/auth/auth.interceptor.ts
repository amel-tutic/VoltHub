import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

const AUTH_ENDPOINTS = ['/api/auth/login', '/api/auth/register', '/api/auth/refresh'];

// Runs for every HTTP request: attaches the access token, and on a 401 refreshes once and retries.
export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const isAuthEndpoint = AUTH_ENDPOINTS.some(url => request.url.startsWith(url));
  const token = auth.accessToken();

  const withToken = (req: HttpRequest<unknown>, accessToken: string | null) =>
    accessToken && !isAuthEndpoint ? req.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } }) : req;

  return next(withToken(request, token)).pipe(
    catchError((error: unknown) => {
      const expired = error instanceof HttpErrorResponse && error.status === 401 && !isAuthEndpoint && token !== null;
      if (!expired) {
        return throwError(() => error);
      }
      return auth.refresh().pipe(
        switchMap(newToken => next(withToken(request, newToken))),
        catchError(refreshError => {
          auth.logout('ended');   // refresh token expired or revoked: back to the login page, with the reason
          return throwError(() => refreshError);
        }));
    }));
};
