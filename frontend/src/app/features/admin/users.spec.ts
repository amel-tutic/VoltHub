import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AdminUser } from '../../core/api/models';
import { Users } from './users';

describe('Users (administration)', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      imports: [Users],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    });
  });

  it('shows one row per account with the right action', async () => {
    const fixture = TestBed.createComponent(Users);
    const accounts: AdminUser[] = [
      { id: '1', firstName: 'Ana', lastName: 'Ilić', email: 'ana@volthub.local', role: 'Operator', isActive: true,
        createdAt: '2026-09-01T10:00:00Z', vehicleCount: 0 },
      { id: '2', firstName: 'Marko', lastName: 'Perić', email: 'marko@example.com', role: 'User', isActive: false,
        createdAt: '2026-09-15T10:00:00Z', vehicleCount: 2 }
    ];
    TestBed.inject(HttpTestingController).expectOne(request => request.url === '/api/users').flush(accounts);
    await fixture.whenStable();

    const rows = (fixture.nativeElement as HTMLElement).querySelectorAll('tbody tr');
    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('Ana Ilić');
    expect(rows[0].textContent).toContain('Deactivate');
    expect(rows[1].textContent).toContain('EV owner');
    expect(rows[1].textContent).toContain('Activate');
  });
});
