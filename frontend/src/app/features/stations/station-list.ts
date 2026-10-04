import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormField, form } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { Router, RouterLink } from '@angular/router';
import { CONNECTOR_TYPES, NearestStation, StationSummary } from '../../core/api/models';
import { StationFilters, StationsService } from '../../core/services/stations.service';
import { NotifyService } from '../../core/ui/notify.service';
import { StationMap } from './station-map';
import { AuthService } from '../../core/auth/auth.service';

const NOVI_SAD = { latitude: 45.2551, longitude: 19.8451 };   // used when the browser can't share a location

@Component({
  selector: 'app-station-list',
  imports: [FormField, RouterLink, DecimalPipe, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule,
            MatButtonModule, MatIconModule, MatProgressBarModule, StationMap],
  templateUrl: './station-list.html',
  styleUrl: './station-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StationList {
  private readonly stationsService = inject(StationsService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  protected readonly auth = inject(AuthService);

  protected readonly connectorTypes = CONNECTOR_TYPES;
  protected readonly stations = signal<StationSummary[]>([]);
  protected readonly nearest = signal<NearestStation[] | null>(null);
  protected readonly loading = signal(false);

  protected readonly filters = signal<StationFilters>({ city: '', connectorType: '', onlyAvailable: false });
  protected readonly filterForm = form(this.filters);

  constructor() {
    this.search();
  }

  protected search(): void {
    this.loading.set(true);
    this.stationsService.search(this.filters()).subscribe({
      next: stations => { this.stations.set(stations); this.loading.set(false); },
      error: error => { this.notify.error(error); this.loading.set(false); }
    });
  }

  // Requirement 2.4: recommend the nearest station with a free, compatible charger.
  protected recommend(): void {
    const connector = this.filters().connectorType || undefined;
    const run = (latitude: number, longitude: number) =>
      this.stationsService.nearest(latitude, longitude, connector).subscribe({
        next: result => this.nearest.set(result),
        error: error => this.notify.error(error)
      });

    if (!navigator.geolocation) {
      run(NOVI_SAD.latitude, NOVI_SAD.longitude);
      return;
    }
    navigator.geolocation.getCurrentPosition(
      position => run(position.coords.latitude, position.coords.longitude),
      () => run(NOVI_SAD.latitude, NOVI_SAD.longitude),
      { timeout: 5000 });
  }

  protected open(stationId: string): void {
    void this.router.navigate(['/stations', stationId]);
  }
}
