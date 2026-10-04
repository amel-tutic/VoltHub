import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { ActiveSession, CreatedResponse, SessionHistoryItem, SessionSummary } from '../api/models';
import { BACKGROUND } from '../ui/loading';

@Injectable({ providedIn: 'root' })
export class SessionsService {
  private readonly http = inject(HttpClient);

  start(reservationId: string) { return this.http.post<CreatedResponse>('/api/sessions/start', { reservationId }); }
  // Polled every 5 seconds by the charging page, so it runs without the loading bar.
  active() { return this.http.get<ActiveSession[]>('/api/sessions/active', { context: new HttpContext().set(BACKGROUND, true) }); }
  stop(sessionId: string) { return this.http.post<SessionSummary>(`/api/sessions/${sessionId}/stop`, {}); }
  history() { return this.http.get<SessionHistoryItem[]>('/api/sessions'); }
}
