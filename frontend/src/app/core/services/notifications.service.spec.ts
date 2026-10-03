import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { NotificationsService } from './notifications.service';

describe('NotificationsService', () => {
  let service: NotificationsService;
  let backend: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(NotificationsService);
    backend = TestBed.inject(HttpTestingController);
  });

  afterEach(() => backend.verify());

  it('keeps the unread count the API reports', () => {
    service.list().subscribe();
    backend.expectOne(request => request.url === '/api/notifications').flush({ unreadCount: 3, items: [] });
    expect(service.unreadCount()).toBe(3);
  });

  it('lowers the count when one is read and clears it when all are read', () => {
    service.unreadCount.set(2);

    service.markRead('n1').subscribe();
    backend.expectOne('/api/notifications/n1/read').flush(null);
    expect(service.unreadCount()).toBe(1);

    service.markAllRead().subscribe();
    backend.expectOne('/api/notifications/read-all').flush(null);
    expect(service.unreadCount()).toBe(0);
  });
});
