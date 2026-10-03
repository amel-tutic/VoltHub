import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { AuthResponse, UserProfile } from '../api/models';

// The logged-in user's own account (SSA 1.3), for every role.
@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly http = inject(HttpClient);

  updateProfile(firstName: string, lastName: string) {
    return this.http.put<UserProfile>('/api/account/profile', { firstName, lastName });
  }

  changePassword(currentPassword: string, newPassword: string) {
    return this.http.post<AuthResponse>('/api/account/password', { currentPassword, newPassword });
  }
}
