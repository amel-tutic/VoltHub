import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { EMPTY, catchError, switchMap, timer } from 'rxjs';
import { AuthService } from './core/auth/auth.service';
import { NotificationsService } from './core/services/notifications.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatButtonModule, MatIconModule, MatBadgeModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class App {
  protected readonly auth = inject(AuthService);
  protected readonly notifications = inject(NotificationsService);

  constructor() {
    // After a page reload the tokens are still stored; fetch who we are for the toolbar.
    if (this.auth.isLoggedIn()) {
      this.auth.loadProfile().subscribe({ error: () => {} });
    }

    // Operators and administrators get a bell with the unread count, refreshed every 30 seconds.
    // The effect re-runs when the role changes (login, logout); onCleanup stops the old timer.
    effect(onCleanup => {
      if (!this.auth.isStaff()) return;
      const polling = timer(0, 30_000)
        .pipe(switchMap(() => this.notifications.refreshUnreadCount().pipe(catchError(() => EMPTY))))
        .subscribe();
      onCleanup(() => polling.unsubscribe());
    });
  }
}
