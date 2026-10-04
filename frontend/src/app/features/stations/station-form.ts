import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { FormField, form, max, maxLength, min, required, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { StationInput } from '../../core/api/models';
import { StationsService } from '../../core/services/stations.service';
import { NotifyService } from '../../core/ui/notify.service';
import { LocationPicker, MapPoint } from './location-picker';

// A new station starts in the centre of Novi Sad; the operator moves the coordinates to the real spot.
const NEW_STATION: StationInput = { name: '', address: '', city: '', latitude: 45.2551, longitude: 19.8451, description: '' };

// SSA 6.1: one form for both creating (/stations/new) and editing (/stations/:id/edit) a station.
@Component({
  selector: 'app-station-form',
  imports: [FormField, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, LocationPicker],
  templateUrl: './station-form.html',
  styleUrl: './station-form.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StationForm {
  // The :id route parameter: missing on /stations/new, present on /stations/:id/edit.
  readonly id = input<string>();

  private readonly stationsService = inject(StationsService);
  private readonly router = inject(Router);
  private readonly notify = inject(NotifyService);

  protected readonly isEdit = computed(() => this.id() !== undefined);
  protected readonly model = signal<StationInput>({ ...NEW_STATION });
  protected readonly stationForm = form(this.model, path => {
    required(path.name, { message: 'Name is required.' });
    maxLength(path.name, 150, { message: 'At most 150 characters.' });
    required(path.address, { message: 'Address is required.' });
    maxLength(path.address, 255, { message: 'At most 255 characters.' });
    required(path.city, { message: 'City is required.' });
    maxLength(path.city, 100, { message: 'At most 100 characters.' });
    min(path.latitude, -90, { message: 'Latitude is between -90 and 90.' });
    max(path.latitude, 90, { message: 'Latitude is between -90 and 90.' });
    min(path.longitude, -180, { message: 'Longitude is between -180 and 180.' });
    max(path.longitude, 180, { message: 'Longitude is between -180 and 180.' });
    maxLength(path.description, 1000, { message: 'At most 1000 characters.' });
  });
  protected readonly serverError = signal<string | null>(null);

  constructor() {
    // Edit mode: load the station and put its current values into the form.
    effect(() => {
      const id = this.id();
      if (id) this.load(id);
    });
  }

  protected onSubmit(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    void submit(this.stationForm, async () => {
      try {
        const id = this.id();
        if (id) {
          await firstValueFrom(this.stationsService.update(id, this.model()));
          this.notify.success('Station saved.');
          await this.router.navigate(['/stations', id]);
        } else {
          const created = await firstValueFrom(this.stationsService.create(this.model()));
          this.notify.success('Station created. Now add its chargers.');
          await this.router.navigate(['/stations', created.id]);
        }
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));
      }
      return undefined;
    });
  }

  // The map sends a clicked point; it goes into the form exactly like typed coordinates.
  protected setLocation(point: MapPoint): void {
    this.model.update(station => ({ ...station, ...point }));
  }

  private load(id: string): void {
    this.stationsService.get(id).subscribe({
      next: station => this.model.set({
        name: station.name, address: station.address, city: station.city,
        latitude: station.latitude, longitude: station.longitude, description: station.description ?? ''
      }),
      error: error => this.notify.error(error)
    });
  }
}
