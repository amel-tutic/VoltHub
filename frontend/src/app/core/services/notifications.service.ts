import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { map, tap } from 'rxjs';
import { NotificationList } from '../api/models';
import { BACKGROUND } from '../ui/loading';

// Notifications (SSA 7.6). The unread count lives here, so the toolbar bell and the
// notifications page always show the same number.
@Injectable({ providedIn: 'root' })
export class NotificationsService {
  private readonly http = inject(HttpClient);

  readonly unreadCount = signal(0);

  list(unreadOnly = false) {
    return this.http.get<NotificationList>('/api/notifications', { params: { unreadOnly } }).pipe(
      tap(list => this.unreadCount.set(list.unreadCount)));
  }

  // A light call for the toolbar: one item is enough, only the count matters.
  refreshUnreadCount() {
    return this.http.get<NotificationList>('/api/notifications', {
      params: { unreadOnly: true, take: 1 }, context: new HttpContext().set(BACKGROUND, true)   // polled: no loading bar
    }).pipe(
      tap(list => this.unreadCount.set(list.unreadCount)),
      map(list => list.unreadCount));
  }

  markRead(id: string) {
    return this.http.post<void>(`/api/notifications/${id}/read`, {}).pipe(
      tap(() => this.unreadCount.update(count => Math.max(0, count - 1))));
  }

  markAllRead() {
    return this.http.post<void>('/api/notifications/read-all', {}).pipe(tap(() => this.unreadCount.set(0)));
  }
}
