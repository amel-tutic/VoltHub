import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { CreatedResponse, ProblemReport, ProblemStatus, ProblemType, StationRatings } from '../api/models';

// Ratings and problem reports (SSA 8.3, 8.4, 8.5).
@Injectable({ providedIn: 'root' })
export class FeedbackService {
  private readonly http = inject(HttpClient);

  ratings(stationId: string) {
    return this.http.get<StationRatings>(`/api/stations/${stationId}/ratings`);
  }

  rate(stationId: string, score: number, comment: string) {
    return this.http.put<void>(`/api/stations/${stationId}/ratings`, { score, comment: comment.trim() || null });
  }

  reportProblem(chargerId: string, type: ProblemType, description: string) {
    return this.http.post<CreatedResponse>('/api/problem-reports', { chargerId, type, description });
  }

  myReports() {
    return this.http.get<ProblemReport[]>('/api/problem-reports/mine');
  }

  allReports(status: ProblemStatus | '') {
    const params = status ? new HttpParams().set('status', status) : undefined;
    return this.http.get<ProblemReport[]>('/api/problem-reports', { params });
  }

  changeStatus(id: string, status: ProblemStatus) {
    return this.http.patch<void>(`/api/problem-reports/${id}/status`, { status });
  }
}
