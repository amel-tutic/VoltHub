import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { apiErrorMessage } from '../api/api-error';

// One place for toast messages, so every page reports success and failure the same way.
@Injectable({ providedIn: 'root' })
export class NotifyService {
  private readonly snackBar = inject(MatSnackBar);

  success(message: string): void {
    this.snackBar.open(message, 'OK', { duration: 3000 });
  }

  error(error: unknown): void {
    this.snackBar.open(apiErrorMessage(error), 'Close', { duration: 6000, panelClass: 'snack-error' });
  }
}
