import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { MyStatistics } from '../api/models';

// The EV owner's consumption statistics (SSA 8.2).
@Injectable({ providedIn: 'root' })
export class StatisticsService {
  private readonly http = inject(HttpClient);

  mine() {
    return this.http.get<MyStatistics>('/api/statistics/me');
  }
}
