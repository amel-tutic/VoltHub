import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';
import { Charger, MyStatistics, PROBLEM_TYPE_LABELS, ProblemReport, ProblemType, SessionHistoryItem } from '../../core/api/models';
import { FeedbackService } from '../../core/services/feedback.service';
import { SessionsService } from '../../core/services/sessions.service';
import { StatisticsService } from '../../core/services/statistics.service';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';

interface ChargerTotal { key: string; stationName: string; chargerCode: string; sessions: number; energyKwh: number; cost: number; }

// SSA 8.2 and 8.4: the owner's consumption statistics and the problems they reported.
@Component({
  selector: 'app-statistics',
  imports: [DatePipe, DecimalPipe, RouterLink, MatCardModule, StatusChip],
  templateUrl: './statistics.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Statistics {
  private readonly statisticsService = inject(StatisticsService);
  private readonly feedbackService = inject(FeedbackService);
  private readonly sessionsService = inject(SessionsService);
  private readonly notify = inject(NotifyService);

  protected readonly stats = signal<MyStatistics | null>(null);
  protected readonly reports = signal<ProblemReport[]>([]);
  protected readonly maxMonthEnergy = computed(() => Math.max(1, ...(this.stats()?.monthly ?? []).map(m => m.energyKwh)));

  private readonly sessions = signal<SessionHistoryItem[]>([]);
  protected readonly byCharger = computed(() => {
    const totals = new Map<string, ChargerTotal>();
    for (const session of this.sessions()) {
      if(session.status !== 'Completed') continue;
      const key = `${session.stationName} ${session.chargerCode}`;
      const total = totals.get(key) ?? {key, stationName: session.stationName, chargerCode: session.chargerCode, sessions: 0, energyKwh: 0, cost: 0};
      total.sessions += 1;
      total.energyKwh += session.energyKwh ?? 0;
      total.cost += session.totalPrice ?? 0;
      totals.set(key, total);
    }
    return [...totals.values()].sort((a, b) => b.energyKwh - a.energyKwh);
  })

  constructor() {
    this.sessionsService.history().subscribe({
      next: sessions => this.sessions.set(sessions),
      error: error => this.notify.error(error)
    });
    this.statisticsService.mine().subscribe({
      next: stats => this.stats.set(stats),
      error: error => this.notify.error(error)
    });
    this.feedbackService.myReports().subscribe({
      next: reports => this.reports.set(reports),
      error: error => this.notify.error(error)
    });
  }

  // "2026-09" -> "Sep 2026"
  protected monthLabel(month: string): string {
    return new Date(`${month}-01T00:00:00`).toLocaleDateString('en', { month: 'short', year: 'numeric' });
  }

  protected widthPercent(value: number, max: number): number {
    return Math.round((value / max) * 100);
  }

  protected typeLabel(type: ProblemType): string {
    return PROBLEM_TYPE_LABELS[type];
  }
}
