import { HttpContextToken, HttpInterceptorFn } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';

// Marks a request as background work (polling) that should not show the loading bar.
// Use: this.http.get(url, { context: new HttpContext().set(BACKGROUND, true) })
export const BACKGROUND = new HttpContextToken<boolean>(() => false);

// Counts the requests in flight; the toolbar shows a thin bar while the count is above zero.
@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly pending = signal(0);
  readonly active = computed(() => this.pending() > 0);

  start(): void { this.pending.update(count => count + 1); }
  stop(): void { this.pending.update(count => Math.max(0, count - 1)); }
}

export const loadingInterceptor: HttpInterceptorFn = (request, next) => {
  if (request.context.get(BACKGROUND)) {
    return next(request);
  }
  const loading = inject(LoadingService);
  loading.start();
  return next(request).pipe(finalize(() => loading.stop()));   // finalize runs on success, error and cancel
};
