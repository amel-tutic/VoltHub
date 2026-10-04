import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import {
  BusySlot, ChargerInput, ChargerStatus, ConnectorType, CreatedResponse, NearestStation, StationDetails, StationInput, StationSummary
} from '../api/models';

export interface StationFilters { city: string; connectorType: ConnectorType | ''; onlyAvailable: boolean; }

@Injectable({ providedIn: 'root' })
export class StationsService {
  private readonly http = inject(HttpClient);

  search(filters: StationFilters) {
    let params = new HttpParams().set('onlyAvailable', filters.onlyAvailable);
    if (filters.city.trim()) params = params.set('city', filters.city.trim());
    if (filters.connectorType) params = params.set('connectorType', filters.connectorType);
    return this.http.get<StationSummary[]>('/api/stations', { params });
  }

  nearest(latitude: number, longitude: number, connectorType?: ConnectorType) {
    let params = new HttpParams().set('latitude', latitude).set('longitude', longitude).set('take', 3);
    if (connectorType) params = params.set('connectorType', connectorType);
    return this.http.get<NearestStation[]>('/api/stations/nearest', { params });
  }

  get(id: string) {
    return this.http.get<StationDetails>(`/api/stations/${id}`);
  }

  schedule(chargerId: string) {
    return this.http.get<BusySlot[]>(`/api/chargers/${chargerId}/schedule`);
  }

  setChargerStatus(chargerId: string, status: ChargerStatus) {
    return this.http.patch<void>(`/api/chargers/${chargerId}/status`, { status });
  }

  setChargerPrice(chargerId: string, pricePerKwh: number) {
    return this.http.patch<void>(`/api/chargers/${chargerId}/price`, { pricePerKwh });
  }

  // ---- Station management (SSA 6.1, 6.2), for operators and administrators

  create(station: StationInput) {
    return this.http.post<CreatedResponse>('/api/stations', this.toBody(station));
  }

  update(id: string, station: StationInput) {
    return this.http.put<void>(`/api/stations/${id}`, this.toBody(station));
  }

  delete(id: string) {
    return this.http.delete<void>(`/api/stations/${id}`);
  }

  addCharger(stationId: string, charger: ChargerInput) {
    return this.http.post<CreatedResponse>(`/api/stations/${stationId}/chargers`, charger);
  }

  // The API stores "no description" as null, not as an empty string.
  private toBody(station: StationInput) {
    return { ...station, description: station.description.trim() || null };
  }
}
