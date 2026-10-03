import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { CreatedResponse, Reservation } from '../api/models';

@Injectable({ providedIn: 'root' })
export class ReservationsService {
  private readonly http = inject(HttpClient);

  mine() { return this.http.get<Reservation[]>('/api/reservations'); }

  // Times are sent as ISO-8601 with a zone ("...Z"), which the API reads as DateTimeOffset.
  create(chargerId: string, vehicleId: string, startTime: Date, endTime: Date) {
    return this.http.post<CreatedResponse>('/api/reservations', {
      chargerId, vehicleId, startTime: startTime.toISOString(), endTime: endTime.toISOString()
    });
  }

  cancel(id: string) { return this.http.post<void>(`/api/reservations/${id}/cancel`, {}); }
}
