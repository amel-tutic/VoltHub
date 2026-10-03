import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RouterLink } from '@angular/router';
import { catchError, of, switchMap, timer } from 'rxjs';
import { ActiveSession, SessionHistoryItem, SessionSummary } from '../../core/api/models';
import { SessionsService } from '../../core/services/sessions.service';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';

@Component({
  selector: 'app-charging',
  imports: [DatePipe, DecimalPipe, RouterLink, MatCardModule, MatButtonModule, MatIconModule, MatProgressBarModule, StatusChip],
  templateUrl: './charging.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Charging {
  private readonly sessionsService = inject(SessionsService);
  private readonly notify = inject(NotifyService);

  protected readonly active = signal<ActiveSession[]>([]);
  protected readonly history = signal<SessionHistoryItem[]>([]);
  protected readonly lastSummary = signal<SessionSummary | null>(null);

  constructor() {
    // Poll the live state every 5 s; the API computes energy and cost "so far" on each call.
    timer(0, 5000)
      .pipe(
        switchMap(() => this.sessionsService.active().pipe(catchError(() => of(null)))),   // keep polling after a failure
        takeUntilDestroyed())
      .subscribe(active => { if (active) this.active.set(active); });
    this.loadHistory();
  }

  protected loadHistory(): void {
    this.sessionsService.history().subscribe({ next: items => this.history.set(items), error: error => this.notify.error(error) });
  }

  protected stop(session: ActiveSession): void {
    this.sessionsService.stop(session.sessionId).subscribe({
      next: summary => {
        this.lastSummary.set(summary);
        this.active.update(list => list.filter(s => s.sessionId !== session.sessionId));
        this.loadHistory();
      },
      error: error => this.notify.error(error)
    });
  }
}
