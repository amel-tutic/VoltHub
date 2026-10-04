import { Routes } from '@angular/router';
import { adminGuard, authGuard, guestGuard, ownerGuard, staffGuard } from './core/auth/auth.guards';

// Each page is loaded only when first visited (lazy loading), keeping the initial download small.
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'stations' },
  { path: 'login', title: 'Log in · VoltHub', canActivate: [guestGuard], loadComponent: () => import('./features/auth/login').then(m => m.Login) },
  { path: 'register', title: 'Register · VoltHub', canActivate: [guestGuard], loadComponent: () => import('./features/auth/register').then(m => m.Register) },
  { path: 'stations', title: 'Stations · VoltHub', canActivate: [authGuard], loadComponent: () => import('./features/stations/station-list').then(m => m.StationList) },
  // 'stations/new' must come before 'stations/:id', or the router would read "new" as a station id.
  { path: 'stations/new', title: 'New station · VoltHub', canActivate: [staffGuard], loadComponent: () => import('./features/stations/station-form').then(m => m.StationForm) },
  { path: 'stations/:id/edit', title: 'Edit station · VoltHub', canActivate: [staffGuard], loadComponent: () => import('./features/stations/station-form').then(m => m.StationForm) },
  { path: 'stations/:id', title: 'Station · VoltHub', canActivate: [authGuard], loadComponent: () => import('./features/stations/station-detail').then(m => m.StationDetail) },
  { path: 'vehicles', title: 'My vehicles · VoltHub', canActivate: [ownerGuard], loadComponent: () => import('./features/vehicles/vehicles').then(m => m.Vehicles) },
  { path: 'reservations', title: 'Reservations · VoltHub', canActivate: [ownerGuard], loadComponent: () => import('./features/reservations/reservations').then(m => m.Reservations) },
  { path: 'charging', title: 'Charging · VoltHub', canActivate: [ownerGuard], loadComponent: () => import('./features/charging/charging').then(m => m.Charging) },
  { path: 'invoices', title: 'Invoices · VoltHub', canActivate: [ownerGuard], loadComponent: () => import('./features/invoices/invoices').then(m => m.Invoices) },
  { path: 'statistics', title: 'Statistics · VoltHub', canActivate: [ownerGuard], loadComponent: () => import('./features/statistics/statistics').then(m => m.Statistics) },
  { path: 'profile', title: 'Profile · VoltHub', canActivate: [authGuard], loadComponent: () => import('./features/account/profile').then(m => m.Profile) },
  { path: 'maintenance', title: 'Maintenance · VoltHub', canActivate: [staffGuard], loadComponent: () => import('./features/operator/maintenance').then(m => m.Maintenance) },
  { path: 'problems', title: 'Problem reports · VoltHub', canActivate: [staffGuard], loadComponent: () => import('./features/operator/problems').then(m => m.Problems) },
  { path: 'notifications', title: 'Notifications · VoltHub', canActivate: [staffGuard], loadComponent: () => import('./features/notifications/notifications').then(m => m.Notifications) },
  { path: 'admin/users', title: 'Users · VoltHub', canActivate: [adminGuard], loadComponent: () => import('./features/admin/users').then(m => m.Users) },
  { path: 'admin/reports', title: 'Reports · VoltHub', canActivate: [adminGuard], loadComponent: () => import('./features/admin/reports').then(m => m.Reports) },
  { path: '**', redirectTo: 'stations' }
];
