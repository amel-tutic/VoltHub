import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, finalize, map, shareReplay, switchMap, tap, throwError } from 'rxjs';
import { AuthResponse, Role, UserProfile } from '../api/models';

interface StoredAuth { accessToken: string; refreshToken: string; role: Role; }

const STORAGE_KEY = 'volthub.auth';

// Holds the login state as signals, so every component showing "who is logged in" updates automatically.
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly stored = signal<StoredAuth | null>(readStorage());
  private refreshInFlight$: Observable<string> | null = null;

  readonly profile = signal<UserProfile | null>(null);
  readonly isLoggedIn = computed(() => this.stored() !== null);
  readonly role = computed(() => this.stored()?.role ?? null);
  readonly isOwner = computed(() => this.role() === 'User');
  readonly isStaff = computed(() => this.role() === 'Operator' || this.role() === 'Admin');
  readonly isAdmin = computed(() => this.role() === 'Admin');

  accessToken(): string | null {
    return this.stored()?.accessToken ?? null;
  }

  login(email: string, password: string): Observable<UserProfile> {
    return this.http.post<AuthResponse>('/api/auth/login', { email, password }).pipe(
      tap(response => this.store(response)),
      switchMap(() => this.loadProfile()));
  }

  register(firstName: string, lastName: string, email: string, password: string): Observable<UserProfile> {
    return this.http.post<AuthResponse>('/api/auth/register', { firstName, lastName, email, password }).pipe(
      tap(response => this.store(response)),
      switchMap(() => this.loadProfile()));
  }

  loadProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>('/api/auth/me').pipe(tap(profile => this.profile.set(profile)));
  }

  // Exchanges the refresh token for a new pair. Concurrent callers share one request,
  // because the API rotates refresh tokens: a second parallel refresh would be treated as reuse.
  refresh(): Observable<string> {
    const refreshToken = this.stored()?.refreshToken;
    if (!refreshToken) {
      return throwError(() => new Error('Not logged in.'));
    }
    this.refreshInFlight$ ??= this.http.post<AuthResponse>('/api/auth/refresh', { refreshToken }).pipe(
      tap(response => this.store(response)),
      map(response => response.accessToken),
      finalize(() => (this.refreshInFlight$ = null)),
      shareReplay(1));
    return this.refreshInFlight$;
  }

  // After a password change the API signs out every other device and returns a fresh pair for this one.
  applyTokens(response: AuthResponse): void {
    this.store(response);
  }

  // reason 'ended': the session ran out (or the account was deactivated); the login page says so.
  logout(reason?: 'ended'): void {
    this.stored.set(null);
    this.profile.set(null);
    localStorage.removeItem(STORAGE_KEY);
    void this.router.navigate(['/login'], { queryParams: reason ? { reason } : {} });
  }

  private store(response: AuthResponse): void {
    const value: StoredAuth = { accessToken: response.accessToken, refreshToken: response.refreshToken, role: response.role };
    this.stored.set(value);
    // Demo trade-off: localStorage survives page reloads but is readable by injected scripts (XSS).
    // Production hardening: refresh token in an httpOnly cookie set by the API.
    localStorage.setItem(STORAGE_KEY, JSON.stringify(value));
  }
}

function readStorage(): StoredAuth | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as StoredAuth) : null;
  } catch {
    return null;
  }
}
