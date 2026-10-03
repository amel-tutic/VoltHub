import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { AdminUser, CreatedResponse, Role } from '../api/models';

export interface UserFilters { role: Role | ''; search: string; }
export interface NewOperator { firstName: string; lastName: string; email: string; password: string; }

// Account administration (SSA 1.5). The API allows these calls for administrators only.
@Injectable({ providedIn: 'root' })
export class UsersService {
  private readonly http = inject(HttpClient);

  list(filters: UserFilters) {
    let params = new HttpParams();
    if (filters.role) params = params.set('role', filters.role);
    if (filters.search.trim()) params = params.set('search', filters.search.trim());
    return this.http.get<AdminUser[]>('/api/users', { params });
  }

  createOperator(operator: NewOperator) {
    return this.http.post<CreatedResponse>('/api/users/operators', operator);
  }

  setActive(id: string, isActive: boolean) {
    return this.http.patch<void>(`/api/users/${id}/active`, { isActive });
  }
}
