import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { AppNotification, NotificationType } from '../../core/api/models';
import { NotificationsService } from '../../core/services/notifications.service';
import { NotifyService } from '../../core/ui/notify.service';

const ICONS: Record<NotificationType, string> = {
  MaintenanceDue: 'event',
  ChargerOffline: 'power_off',
  NewProblemReport: 'report_problem'
};

// SSA 7.6: the operator reads notifications created by the notification job (SSA 7.4).
@Component({
  selector: 'app-notifications',
  imports: [DatePipe, MatCardModule, MatButtonModule, MatIconModule, MatSlideToggleModule],
  templateUrl: './notifications.html',
  styleUrl: './notifications.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Notifications {
  private readonly notificationsService = inject(NotificationsService);
  private readonly notify = inject(NotifyService);

  protected readonly unreadOnly = signal(false);
  protected readonly items = signal<AppNotification[]>([]);
  protected readonly unreadCount = this.notificationsService.unreadCount;

  constructor() {
    this.load();
  }

  protected load(): void {
    this.notificationsService.list(this.unreadOnly()).subscribe({
      next: list => this.items.set(list.items),
      error: error => this.notify.error(error)
    });
  }

  protected setUnreadOnly(value: boolean): void {
    this.unreadOnly.set(value);
    this.load();
  }

  protected icon(type: NotificationType): string {
    return ICONS[type];
  }

  protected markRead(item: AppNotification): void {
    if (item.isRead) return;
    this.notificationsService.markRead(item.id).subscribe({
      // Update the one item locally instead of reloading the whole list.
      next: () => this.items.update(items => items.map(i => (i.id === item.id ? { ...i, isRead: true } : i))),
      error: error => this.notify.error(error)
    });
  }

  protected markAllRead(): void {
    this.notificationsService.markAllRead().subscribe({
      next: () => this.load(),
      error: error => this.notify.error(error)
    });
  }
}
