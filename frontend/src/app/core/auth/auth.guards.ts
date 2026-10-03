import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

// Guards only improve navigation; the API enforces every permission on its own.
export const authGuard: CanActivateFn = () =>
  inject(AuthService).isLoggedIn() || inject(Router).createUrlTree(['/login']);

export const guestGuard: CanActivateFn = () =>
  !inject(AuthService).isLoggedIn() || inject(Router).createUrlTree(['/stations']);

export const ownerGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }
  return auth.isOwner() || router.createUrlTree(['/stations']);
};

// Operators and administrators: maintenance, problem reports, notifications.
export const staffGuard: CanActivateFn = () => roleGuard(auth => auth.isStaff());

// Administrators only: accounts and reports.
export const adminGuard: CanActivateFn = () => roleGuard(auth => auth.isAdmin());

function roleGuard(allowed: (auth: AuthService) => boolean) {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }
  return allowed(auth) || router.createUrlTree(['/stations']);
}
