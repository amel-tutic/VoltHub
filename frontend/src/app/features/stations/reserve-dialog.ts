import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormField, form, required, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { BusySlot, Charger, Vehicle } from '../../core/api/models';
import { ReservationsService } from '../../core/services/reservations.service';
import { StationsService } from '../../core/services/stations.service';
import { VehiclesService } from '../../core/services/vehicles.service';
import { addMinutes, nextQuarterHour, toLocalInput } from '../../core/ui/datetime';

export interface ReserveDialogData { charger: Charger; stationName: string; }

@Component({
  selector: 'app-reserve-dialog',
  imports: [FormField, DatePipe, RouterLink, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  templateUrl: './reserve-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReserveDialog {
  protected readonly data = inject<ReserveDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<ReserveDialog, boolean>);
  private readonly vehiclesService = inject(VehiclesService);
  private readonly reservationsService = inject(ReservationsService);
  private readonly stationsService = inject(StationsService);

  protected readonly vehicles = signal<Vehicle[]>([]);
  protected readonly loaded = signal(false);
  protected readonly busySlots = signal<BusySlot[]>([]);
  protected readonly serverError = signal<string | null>(null);
  // Only vehicles whose plug fits this charger (the API checks it too).
  protected readonly compatible = computed(() =>
    this.vehicles().filter(v => v.connectorType === this.data.charger.connectorType));

  private readonly defaultStart = nextQuarterHour();
  protected readonly model = signal({
    vehicleId: '',
    start: toLocalInput(this.defaultStart),
    end: toLocalInput(addMinutes(this.defaultStart, 60))
  });
  protected readonly reserveForm = form(this.model, path => {
    required(path.vehicleId, { message: 'Choose one of your vehicles.' });
    required(path.start, { message: 'Start is required.' });
    required(path.end, { message: 'End is required.' });
  });

  constructor() {
    this.vehiclesService.mine().subscribe({
      next: vehicles => {
        this.vehicles.set(vehicles);
        this.loaded.set(true);
        const first = vehicles.find(v => v.connectorType === this.data.charger.connectorType);
        if (first) this.model.update(m => ({ ...m, vehicleId: first.id }));
      },
      error: error => this.serverError.set(apiErrorMessage(error))
    });
    this.stationsService.schedule(this.data.charger.id).subscribe({ next: slots => this.busySlots.set(slots) });
  }

  // "Now" for a demo booking: start in the current minute, keep the chosen end.
  protected startNow(): void {
    this.model.update(m => ({ ...m, start: toLocalInput(new Date()) }));
  }

  protected onSubmit(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    void submit(this.reserveForm, async () => {
      const { vehicleId, start, end } = this.model();
      try {
        await firstValueFrom(this.reservationsService.create(this.data.charger.id, vehicleId, new Date(start), new Date(end)));
        this.dialogRef.close(true);
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));   // e.g. 409 "already reserved for an overlapping time slot"
      }
      return undefined;
    });
  }
}
