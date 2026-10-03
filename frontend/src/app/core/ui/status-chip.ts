import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

// Colored label for any status the API returns (charger, reservation, session, invoice, report, maintenance).
const TONE: Record<string, string> = {
  Available: 'ok', Completed: 'ok', Paid: 'ok', Resolved: 'ok', Repair: 'ok',
  Reserved: 'warn', Pending: 'warn', Open: 'warn',
  Occupied: 'info', Active: 'info', InProgress: 'info', ScheduledService: 'info',
  OutOfOrder: 'bad', Failed: 'bad', Fault: 'bad',
  UnderMaintenance: 'muted', Cancelled: 'muted', Expired: 'muted', Inspection: 'muted', Inactive: 'muted'
};

const LABEL: Record<string, string> = {
  OutOfOrder: 'Out of order', UnderMaintenance: 'Under maintenance', InProgress: 'In progress',
  ScheduledService: 'Planned service'
};

@Component({
  selector: 'app-status-chip',
  template: `<span class="chip" [class]="'chip chip--' + tone()">{{ label() }}</span>`,
  styles: `
    .chip { display: inline-block; padding: 2px 10px; border-radius: 12px; font-size: 12px; font-weight: 500; }
    .chip--ok { background: #dff3e4; color: #1e6b34; }
    .chip--warn { background: #fff1d6; color: #8a5a00; }
    .chip--info { background: #dde9fb; color: #1f4f96; }
    .chip--bad { background: #fbe0e0; color: #9a1f1f; }
    .chip--muted { background: #ececec; color: #555; }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StatusChip {
  readonly status = input.required<string>();
  protected readonly tone = computed(() => TONE[this.status()] ?? 'muted');
  protected readonly label = computed(() => LABEL[this.status()] ?? this.status());
}
