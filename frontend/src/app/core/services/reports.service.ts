import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { FaultReport, OverviewReport, RevenueReport, StationUsageReport } from '../api/models';

export interface ReportPeriod { from: Date; to: Date; }

// Administrator reports (SSA 9). Every period-based report takes the same from/to range.
@Injectable({ providedIn: 'root' })
export class ReportsService {
  private readonly http = inject(HttpClient);

  overview() {
    return this.http.get<OverviewReport>('/api/reports/overview');
  }

  stations(period: ReportPeriod) {
    return this.http.get<StationUsageReport[]>('/api/reports/stations', { params: toParams(period) });
  }

  revenue(period: ReportPeriod) {
    return this.http.get<RevenueReport>('/api/reports/revenue', { params: toParams(period) });
  }

  faults(period: ReportPeriod) {
    return this.http.get<FaultReport[]>('/api/reports/faults', { params: toParams(period) });
  }
}

function toParams(period: ReportPeriod): HttpParams {
  return new HttpParams().set('from', period.from.toISOString()).set('to', period.to.toISOString());
}
