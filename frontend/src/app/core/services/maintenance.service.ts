import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { ChargerMaintenance, CreatedResponse, OpenMaintenanceItem } from '../api/models';

// Charger maintenance (SSA 7.1, 7.2, 7.3, 7.5) for operators and administrators.
@Injectable({ providedIn: 'root' })
export class MaintenanceService {
  private readonly http = inject(HttpClient);

  open() {
    return this.http.get<OpenMaintenanceItem[]>('/api/maintenance/open');
  }

  history(chargerId: string) {
    return this.http.get<ChargerMaintenance>(`/api/chargers/${chargerId}/maintenance`);
  }

  reportFault(chargerId: string, description: string) {
    return this.http.post<CreatedResponse>(`/api/chargers/${chargerId}/maintenance/faults`, { description });
  }

  plan(chargerId: string, scheduledDate: Date, description: string) {
    return this.http.post<CreatedResponse>(`/api/chargers/${chargerId}/maintenance/planned`,
      { scheduledDate: scheduledDate.toISOString(), description });
  }

  logIntervention(chargerId: string, type: 'Repair' | 'Inspection', description: string) {
    return this.http.post<CreatedResponse>(`/api/chargers/${chargerId}/maintenance/interventions`, { type, description });
  }

  resolve(recordId: string) {
    return this.http.post<void>(`/api/maintenance/${recordId}/resolve`, {});
  }
}
