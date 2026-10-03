import { HttpErrorResponse } from '@angular/common/http';
import { ProblemDetails } from './models';

// Turns any HTTP failure into one sentence a user can act on.
export function apiErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) {
      return 'The server is not reachable. Is the API running?';
    }
    const problem = error.error as ProblemDetails | null;
    const firstFieldError = problem?.errors ? Object.values(problem.errors)[0]?.[0] : undefined;
    return firstFieldError ?? problem?.detail ?? problem?.title ?? `Request failed (${error.status}).`;
  }
  return 'Something went wrong.';
}
