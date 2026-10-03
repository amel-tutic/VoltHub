import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { ActiveSession, CreatedResponse, SessionHistoryItem, SessionSummary } from '../api/models';

@Injectable({ providedIn: 'root' })
export class SessionsService {
  private readonly http = inject(HttpClient);

  start(reservationId: string) { return this.http.post<CreatedResponse>('/api/sessions/start', { reservationId }); }
  active() { return this.http.get<ActiveSession[]>('/api/sessions/active'); }
  stop(sessionId: string) { return this.http.post<SessionSummary>(`/api/sessions/${sessionId}/stop`, {}); }
  history() { return this.http.get<SessionHistoryItem[]>('/api/sessions'); }
}
