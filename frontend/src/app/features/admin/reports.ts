import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { forkJoin } from 'rxjs';
import { FaultReport, OverviewReport, RevenueReport, StationUsageReport } from '../../core/api/models';
import { ReportPeriod, ReportsService } from '../../core/services/reports.service';
import { addDays, toDateInput } from '../../core/ui/datetime';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';

// SSA 9.1–9.4: the administrator's reports for a chosen period (the last 30 days by default).
@Component({
  selector: 'app-reports',
  imports: [DatePipe, DecimalPipe, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, StatusChip],
  templateUrl: './reports.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Reports {
  private readonly reportsService = inject(ReportsService);
  private readonly notify = inject(NotifyService);

  // The two date inputs work with "YYYY-MM-DD" strings.
  protected readonly from = signal(toDateInput(addDays(new Date(), -30)));
  protected readonly to = signal(toDateInput(new Date()));

  protected readonly overview = signal<OverviewReport | null>(null);
  protected readonly stations = signal<StationUsageReport[]>([]);
  protected readonly revenue = signal<RevenueReport | null>(null);
  protected readonly faults = signal<FaultReport[]>([]);

  // Bars are scaled to the largest value, so the biggest row fills the whole width.
  protected readonly maxDayRevenue = computed(() => Math.max(1, ...(this.revenue()?.byDay ?? []).map(d => d.revenue)));
  protected readonly maxUtilization = computed(() => Math.max(1, ...this.stations().map(s => s.utilizationPercent)));

  protected readonly cities = computed(() => [...new Set(this.stations().map(s => s.city))].sort());
  protected readonly city = signal('');
  protected readonly selectedCity = computed(() => this.city() || this.cities()[0] || '');
  protected readonly topInCity = computed(() => this.stations()
    .filter(s => s.city === this.selectedCity())
    .sort((a, b) => b.utilizationPercent - a.utilizationPercent)
    .slice(0, 3));

  constructor() {
    this.load();
  }

  protected load(): void {
    const period = this.period();
    // All four reports in parallel; forkJoin waits until every one has answered.
    forkJoin({
      overview: this.reportsService.overview(),
      stations: this.reportsService.stations(period),
      revenue: this.reportsService.revenue(period),
      faults: this.reportsService.faults(period)
    }).subscribe({
      next: result => {
        this.overview.set(result.overview);
        this.stations.set(result.stations);
        this.revenue.set(result.revenue);
        this.faults.set(result.faults);
      },
      error: error => this.notify.error(error)
    });
  }

  protected setPeriod(from: string, to: string): void {
    if (!from || !to) return;
    this.from.set(from);
    this.to.set(to);
    this.load();
  }

  protected widthPercent(value: number, max: number): number {
    return Math.round((value / max) * 100);
  }

  // The whole "from" day and the whole "to" day are included.
  private period(): ReportPeriod {
    return { from: new Date(`${this.from()}T00:00:00`), to: new Date(`${this.to()}T23:59:59`) };
  }
}
