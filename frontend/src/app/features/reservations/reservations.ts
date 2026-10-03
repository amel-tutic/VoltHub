import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { interval } from 'rxjs';
import { Reservation } from '../../core/api/models';
import { ReservationsService } from '../../core/services/reservations.service';
import { SessionsService } from '../../core/services/sessions.service';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';

const EARLY_START_MS = 5 * 60_000;   // the API lets charging start up to 5 minutes early

@Component({
  selector: 'app-reservations',
  imports: [DatePipe, RouterLink, MatCardModule, MatButtonModule, MatIconModule, StatusChip],
  templateUrl: './reservations.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Reservations {
  private readonly reservationsService = inject(ReservationsService);
  private readonly sessionsService = inject(SessionsService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);

  protected readonly reservations = signal<Reservation[]>([]);
  protected readonly now = signal(Date.now());

  constructor() {
    this.load();
    // Re-evaluate the buttons every 30 s, so "Start charging" appears when the slot begins.
    interval(30_000).pipe(takeUntilDestroyed()).subscribe(() => this.now.set(Date.now()));
  }

  protected load(): void {
    this.reservationsService.mine().subscribe({
      next: reservations => this.reservations.set(reservations),
      error: error => this.notify.error(error)
    });
  }

  // Mirrors the API rules, only to decide which buttons to show; the API decides for real.
  protected canStart(reservation: Reservation): boolean {
    const now = this.now();
    return reservation.status === 'Active'
      && now >= Date.parse(reservation.startTime) - EARLY_START_MS
      && now < Date.parse(reservation.endTime);
  }

  protected canCancel(reservation: Reservation): boolean {
    return reservation.status === 'Active' && this.now() < Date.parse(reservation.startTime);
  }

  protected start(reservation: Reservation): void {
    this.sessionsService.start(reservation.id).subscribe({
      next: () => { this.notify.success('Charging started.'); void this.router.navigateByUrl('/charging'); },
      error: error => this.notify.error(error)
    });
  }

  protected cancel(reservation: Reservation): void {
    if (!confirm(`Cancel the reservation at ${reservation.stationName}?`)) return;
    this.reservationsService.cancel(reservation.id).subscribe({
      next: () => { this.notify.success('Reservation cancelled.'); this.load(); },
      error: error => this.notify.error(error)
    });
  }
}
