import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { authInterceptor } from './auth.interceptor';

describe('authInterceptor', () => {
  let http: HttpClient;
  let backend: HttpTestingController;

  beforeEach(() => {
    localStorage.setItem('volthub.auth', JSON.stringify({ accessToken: 'old-access', refreshToken: 'refresh-1', role: 'User' }));
    TestBed.configureTestingModule({
      providers: [provideRouter([]), provideHttpClient(withInterceptors([authInterceptor])), provideHttpClientTesting()]
    });
    http = TestBed.inject(HttpClient);
    backend = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    backend.verify();
    localStorage.clear();
  });

  it('attaches the access token to API calls', () => {
    http.get('/api/vehicles').subscribe();
    const request = backend.expectOne('/api/vehicles');
    expect(request.request.headers.get('Authorization')).toBe('Bearer old-access');
    request.flush([]);
  });

  it('does not attach a token to the login call', () => {
    http.post('/api/auth/login', {}).subscribe();
    const request = backend.expectOne('/api/auth/login');
    expect(request.request.headers.has('Authorization')).toBe(false);
    request.flush({});
  });

  it('refreshes once on 401 and retries with the new token', () => {
    let result: unknown;
    http.get('/api/vehicles').subscribe(body => (result = body));

    backend.expectOne('/api/vehicles').flush(null, { status: 401, statusText: 'Unauthorized' });

    const refresh = backend.expectOne('/api/auth/refresh');
    expect(refresh.request.body).toEqual({ refreshToken: 'refresh-1' });
    refresh.flush({ accessToken: 'new-access', accessTokenExpiresAt: '', refreshToken: 'refresh-2', role: 'User' });

    const retry = backend.expectOne('/api/vehicles');
    expect(retry.request.headers.get('Authorization')).toBe('Bearer new-access');
    retry.flush([{ id: 'v1' }]);

    expect(result).toEqual([{ id: 'v1' }]);
    expect(JSON.parse(localStorage.getItem('volthub.auth')!).refreshToken).toBe('refresh-2');   // rotated token stored
  });
});
