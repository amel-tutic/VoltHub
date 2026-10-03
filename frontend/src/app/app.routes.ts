import { Routes } from '@angular/router';
import { adminGuard, authGuard, guestGuard, ownerGuard, staffGuard } from './core/auth/auth.guards';

// Each page is loaded only when first visited (lazy loading), keeping the initial download small.
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'stations' },
  { path: 'login', canActivate: [guestGuard], loadComponent: () => import('./features/auth/login').then(m => m.Login) },
  { path: 'register', canActivate: [guestGuard], loadComponent: () => import('./features/auth/register').then(m => m.Register) },
  { path: 'stations', canActivate: [authGuard], loadComponent: () => import('./features/stations/station-list').then(m => m.StationList) },
  { path: 'stations/:id', canActivate: [authGuard], loadComponent: () => import('./features/stations/station-detail').then(m => m.StationDetail) },
  { path: 'vehicles', canActivate: [ownerGuard], loadComponent: () => import('./features/vehicles/vehicles').then(m => m.Vehicles) },
  { path: 'reservations', canActivate: [ownerGuard], loadComponent: () => import('./features/reservations/reservations').then(m => m.Reservations) },
  { path: 'charging', canActivate: [ownerGuard], loadComponent: () => import('./features/charging/charging').then(m => m.Charging) },
  { path: 'invoices', canActivate: [ownerGuard], loadComponent: () => import('./features/invoices/invoices').then(m => m.Invoices) },
  { path: 'statistics', canActivate: [ownerGuard], loadComponent: () => import('./features/statistics/statistics').then(m => m.Statistics) },
  { path: 'profile', canActivate: [authGuard], loadComponent: () => import('./features/account/profile').then(m => m.Profile) },
  { path: 'maintenance', canActivate: [staffGuard], loadComponent: () => import('./features/operator/maintenance').then(m => m.Maintenance) },
  { path: 'problems', canActivate: [staffGuard], loadComponent: () => import('./features/operator/problems').then(m => m.Problems) },
  { path: 'notifications', canActivate: [staffGuard], loadComponent: () => import('./features/notifications/notifications').then(m => m.Notifications) },
  { path: 'admin/users', canActivate: [adminGuard], loadComponent: () => import('./features/admin/users').then(m => m.Users) },
  { path: 'admin/reports', canActivate: [adminGuard], loadComponent: () => import('./features/admin/reports').then(m => m.Reports) },
  { path: '**', redirectTo: 'stations' }
];
