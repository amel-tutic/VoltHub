import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormField, form, maxLength, required, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { Charger, ChargerMaintenance, OpenMaintenanceItem, StationSummary } from '../../core/api/models';
import { MaintenanceService } from '../../core/services/maintenance.service';
import { StationsService } from '../../core/services/stations.service';
import { addDays, toLocalInput } from '../../core/ui/datetime';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';

type MaintenanceAction = 'fault' | 'plan' | 'Repair' | 'Inspection';

interface ActionModel { action: MaintenanceAction; description: string; scheduledDate: string; }

const EMPTY_ACTION = (): ActionModel => ({ action: 'fault', description: '', scheduledDate: toLocalInput(addDays(new Date(), 7)) });

// SSA 7.1–7.3 and 7.5: open work across the network, and one charger's history with the actions on it.
@Component({
  selector: 'app-maintenance',
  imports: [FormField, DatePipe, MatCardModule, MatButtonModule, MatIconModule, MatFormFieldModule, MatInputModule,
            MatSelectModule, StatusChip],
  templateUrl: './maintenance.html',
  styleUrl: './maintenance.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Maintenance {
  private readonly maintenanceService = inject(MaintenanceService);
  private readonly stationsService = inject(StationsService);
  private readonly notify = inject(NotifyService);

  protected readonly openItems = signal<OpenMaintenanceItem[]>([]);
  protected readonly stations = signal<StationSummary[]>([]);
  protected readonly stationId = signal('');
  protected readonly chargers = signal<Charger[]>([]);
  protected readonly chargerId = signal('');
  protected readonly history = signal<ChargerMaintenance | null>(null);

  protected readonly actionModel = signal<ActionModel>(EMPTY_ACTION());
  protected readonly actionForm = form(this.actionModel, path => {
    required(path.description, { message: 'Describe the problem or the work done.' });
    maxLength(path.description, 1000, { message: 'At most 1000 characters.' });
    // Only "plan a service" needs a date: the rule applies when the condition is true.
    required(path.scheduledDate, { message: 'Choose when the service is planned.', when: ({ valueOf }) => valueOf(path.action) === 'plan' });
  });
  protected readonly serverError = signal<string | null>(null);

  constructor() {
    this.loadOpenItems();
    this.stationsService.search({ city: '', connectorType: '', onlyAvailable: false }).subscribe({
      next: stations => this.stations.set(stations),
      error: error => this.notify.error(error)
    });
  }

  protected loadOpenItems(): void {
    this.maintenanceService.open().subscribe({
      next: items => this.openItems.set(items),
      error: error => this.notify.error(error)
    });
  }

  protected selectStation(stationId: string, chargerId = ''): void {
    this.stationId.set(stationId);
    this.chargerId.set('');
    this.history.set(null);
    this.stationsService.get(stationId).subscribe({
      next: station => {
        this.chargers.set(station.chargers);
        if (chargerId) this.selectCharger(chargerId);
      },
      error: error => this.notify.error(error)
    });
  }

  protected selectCharger(chargerId: string): void {
    this.chargerId.set(chargerId);
    this.loadHistory();
  }

  // "Open" on a to-do item jumps to that charger's history below.
  protected openCharger(item: OpenMaintenanceItem): void {
    this.selectStation(item.stationId, item.chargerId);
  }

  protected loadHistory(): void {
    const chargerId = this.chargerId();
    if (!chargerId) return;
    this.maintenanceService.history(chargerId).subscribe({
      next: history => this.history.set(history),
      error: error => this.notify.error(error)
    });
  }

  protected resolve(recordId: string): void {
    this.maintenanceService.resolve(recordId).subscribe({
      next: () => {
        this.notify.success('Marked as resolved.');
        this.loadOpenItems();
        this.loadHistory();
      },
      error: error => this.notify.error(error)
    });
  }

  protected onSubmit(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    const chargerId = this.chargerId();
    if (!chargerId) return;
    void submit(this.actionForm, async () => {
      const { action, description, scheduledDate } = this.actionModel();
      try {
        // One form, four API calls: the chosen action decides which one.
        if (action === 'fault') {
          await firstValueFrom(this.maintenanceService.reportFault(chargerId, description));
        } else if (action === 'plan') {
          await firstValueFrom(this.maintenanceService.plan(chargerId, new Date(scheduledDate), description));
        } else {
          await firstValueFrom(this.maintenanceService.logIntervention(chargerId, action, description));
        }
        this.notify.success('Saved.');
        this.actionModel.set(EMPTY_ACTION());
        this.actionForm().reset();
        this.loadOpenItems();
        this.loadHistory();
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));
      }
      return undefined;
    });
  }
}
