import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';
import { MyStatistics, PROBLEM_TYPE_LABELS, ProblemReport, ProblemType } from '../../core/api/models';
import { FeedbackService } from '../../core/services/feedback.service';
import { StatisticsService } from '../../core/services/statistics.service';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';

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
  private readonly notify = inject(NotifyService);

  protected readonly stats = signal<MyStatistics | null>(null);
  protected readonly reports = signal<ProblemReport[]>([]);
  protected readonly maxMonthEnergy = computed(() => Math.max(1, ...(this.stats()?.monthly ?? []).map(m => m.energyKwh)));

  constructor() {
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
