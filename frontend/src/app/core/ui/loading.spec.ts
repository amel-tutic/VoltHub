import { HttpClient, HttpContext, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { BACKGROUND, LoadingService, loadingInterceptor } from './loading';

describe('loadingInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [provideHttpClient(withInterceptors([loadingInterceptor])), provideHttpClientTesting()]
  }));

  it('is active while a request runs, and not for background requests', () => {
    const http = TestBed.inject(HttpClient);
    const loading = TestBed.inject(LoadingService);
    const backend = TestBed.inject(HttpTestingController);

    http.get('/api/background', { context: new HttpContext().set(BACKGROUND, true) }).subscribe();
    expect(loading.active()).toBe(false);

    http.get('/api/stations').subscribe();
    expect(loading.active()).toBe(true);

    backend.expectOne('/api/stations').flush([]);
    expect(loading.active()).toBe(false);
    backend.expectOne('/api/background').flush([]);
  });
});
