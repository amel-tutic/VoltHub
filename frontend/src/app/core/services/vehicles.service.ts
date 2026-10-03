import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { ConnectorType, CreatedResponse, Vehicle } from '../api/models';

export interface VehicleInput { make: string; model: string; batteryCapacityKwh: number; connectorType: ConnectorType; }

@Injectable({ providedIn: 'root' })
export class VehiclesService {
  private readonly http = inject(HttpClient);

  mine() { return this.http.get<Vehicle[]>('/api/vehicles'); }
  add(vehicle: VehicleInput) { return this.http.post<CreatedResponse>('/api/vehicles', vehicle); }
  remove(id: string) { return this.http.delete<void>(`/api/vehicles/${id}`); }
}
