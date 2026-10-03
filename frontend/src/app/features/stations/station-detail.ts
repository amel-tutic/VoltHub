import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, effect, inject, input, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { RouterLink } from '@angular/router';
import { Charger, ChargerStatus, OPERATOR_STATUSES, StationDetails } from '../../core/api/models';
import { AuthService } from '../../core/auth/auth.service';
import { StationsService } from '../../core/services/stations.service';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';
import { ReportProblemDialog, ReportProblemDialogData } from './report-problem-dialog';
import { ReserveDialog, ReserveDialogData } from './reserve-dialog';
import { StationRatingsPanel } from './station-ratings';

@Component({
  selector: 'app-station-detail',
  imports: [RouterLink, DecimalPipe, MatCardModule, MatButtonModule, MatIconModule, MatFormFieldModule,
            MatInputModule, MatSelectModule, StatusChip, StationRatingsPanel],
  templateUrl: './station-detail.html',
  styleUrl: './station-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StationDetail {
  readonly id = input.required<string>();   // the :id route parameter (withComponentInputBinding)

  private readonly stationsService = inject(StationsService);
  private readonly dialog = inject(MatDialog);
  private readonly notify = inject(NotifyService);
  protected readonly auth = inject(AuthService);

  protected readonly operatorStatuses = OPERATOR_STATUSES;
  protected readonly station = signal<StationDetails | null>(null);

  constructor() {
    effect(() => this.load(this.id()));   // runs again if the route id changes
  }

  protected load(id: string): void {
    this.stationsService.get(id).subscribe({
      next: station => this.station.set(station),
      error: error => this.notify.error(error)
    });
  }

  protected canReserve(charger: Charger): boolean {
    return charger.status !== 'OutOfOrder' && charger.status !== 'UnderMaintenance';
  }

  // Occupied/Reserved are derived by the API; the stored (base) status behind them is Available.
  protected baseStatus(charger: Charger): ChargerStatus {
    return charger.status === 'Occupied' || charger.status === 'Reserved' ? 'Available' : charger.status;
  }

  protected reserve(charger: Charger): void {
    const station = this.station();
    if (!station) return;
    const data: ReserveDialogData = { charger, stationName: station.name };
    this.dialog.open(ReserveDialog, { data, width: '560px' }).afterClosed().subscribe(created => {
      if (created) {
        this.notify.success(`${charger.code} reserved.`);
        this.load(station.id);
      }
    });
  }

  protected reportProblem(charger: Charger): void {
    const station = this.station();
    if (!station) return;
    const data: ReportProblemDialogData = { charger, stationName: station.name };
    this.dialog.open(ReportProblemDialog, { data, width: '520px' }).afterClosed().subscribe(sent => {
      if (sent) this.notify.success('Thank you. The operators have been notified.');
    });
  }

  protected changeStatus(charger: Charger, status: ChargerStatus): void {
    this.stationsService.setChargerStatus(charger.id, status).subscribe({
      next: () => { this.notify.success(`${charger.code} is now ${status}.`); this.load(this.id()); },
      error: error => { this.notify.error(error); this.load(this.id()); }
    });
  }

  protected changePrice(charger: Charger, price: number): void {
    if (Number.isNaN(price)) return;
    this.stationsService.setChargerPrice(charger.id, price).subscribe({
      next: () => { this.notify.success(`${charger.code}: ${price} RSD/kWh.`); this.load(this.id()); },
      error: error => this.notify.error(error)
    });
  }
}
