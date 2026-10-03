import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormField, form, max, min, required, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { CONNECTOR_TYPES, Vehicle } from '../../core/api/models';
import { VehicleInput, VehiclesService } from '../../core/services/vehicles.service';
import { NotifyService } from '../../core/ui/notify.service';

const EMPTY_VEHICLE: VehicleInput = { make: '', model: '', batteryCapacityKwh: 60, connectorType: 'CCS' };

@Component({
  selector: 'app-vehicles',
  imports: [FormField, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule],
  templateUrl: './vehicles.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Vehicles {
  private readonly vehiclesService = inject(VehiclesService);
  private readonly notify = inject(NotifyService);

  protected readonly connectorTypes = CONNECTOR_TYPES;
  protected readonly vehicles = signal<Vehicle[]>([]);
  protected readonly serverError = signal<string | null>(null);

  protected readonly model = signal<VehicleInput>({ ...EMPTY_VEHICLE });
  protected readonly vehicleForm = form(this.model, path => {
    required(path.make, { message: 'Make is required.' });
    required(path.model, { message: 'Model is required.' });
    min(path.batteryCapacityKwh, 1, { message: 'Battery capacity must be at least 1 kWh.' });
    max(path.batteryCapacityKwh, 1000, { message: 'Battery capacity looks too large.' });
  });

  constructor() {
    this.load();
  }

  protected load(): void {
    this.vehiclesService.mine().subscribe({
      next: vehicles => this.vehicles.set(vehicles),
      error: error => this.notify.error(error)
    });
  }

  protected onSubmit(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    void submit(this.vehicleForm, async () => {
      try {
        await firstValueFrom(this.vehiclesService.add(this.model()));
        this.notify.success('Vehicle added.');
        this.model.set({ ...EMPTY_VEHICLE });
        this.vehicleForm().reset();        // clear "touched" so the empty form shows no errors
        this.load();
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));
      }
      return undefined;
    });
  }

  protected remove(vehicle: Vehicle): void {
    if (!confirm(`Delete ${vehicle.make} ${vehicle.model}?`)) return;
    this.vehiclesService.remove(vehicle.id).subscribe({
      next: () => { this.notify.success('Vehicle deleted.'); this.load(); },
      error: error => this.notify.error(error)   // 409 if it has reservations: history is kept
    });
  }
}
