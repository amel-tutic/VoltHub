import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { RouterLink } from '@angular/router';
import { PROBLEM_TYPE_LABELS, ProblemReport, ProblemStatus, ProblemType } from '../../core/api/models';
import { FeedbackService } from '../../core/services/feedback.service';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';

// SSA 8.5: operators review the drivers' problem reports and move them to "in progress" and "resolved".
@Component({
  selector: 'app-problems',
  imports: [DatePipe, RouterLink, MatCardModule, MatButtonModule, MatButtonToggleModule, StatusChip],
  templateUrl: './problems.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Problems {
  private readonly feedbackService = inject(FeedbackService);
  private readonly notify = inject(NotifyService);

  protected readonly status = signal<ProblemStatus | ''>('Open');
  protected readonly reports = signal<ProblemReport[]>([]);

  constructor() {
    this.load();
  }

  protected load(): void {
    this.feedbackService.allReports(this.status()).subscribe({
      next: reports => this.reports.set(reports),
      error: error => this.notify.error(error)
    });
  }

  protected setStatus(status: ProblemStatus | ''): void {
    this.status.set(status);
    this.load();
  }

  protected typeLabel(type: ProblemType): string {
    return PROBLEM_TYPE_LABELS[type];
  }

  protected changeStatus(report: ProblemReport, status: ProblemStatus): void {
    this.feedbackService.changeStatus(report.id, status).subscribe({
      next: () => {
        this.notify.success(status === 'Resolved' ? 'Report resolved.' : 'Report marked as in progress.');
        this.load();
      },
      error: error => this.notify.error(error)
    });
  }
}
