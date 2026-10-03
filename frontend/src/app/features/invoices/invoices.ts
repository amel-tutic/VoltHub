import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { Invoice, PAYMENT_METHODS, PaymentMethod } from '../../core/api/models';
import { InvoicesService } from '../../core/services/invoices.service';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';

const METHOD_LABEL: Record<PaymentMethod, string> = { Card: 'Payment card', EWallet: 'E-wallet', Subscription: 'Subscription' };

@Component({
  selector: 'app-invoices',
  imports: [DatePipe, DecimalPipe, MatCardModule, MatButtonModule, MatFormFieldModule, MatSelectModule, StatusChip],
  templateUrl: './invoices.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Invoices {
  private readonly invoicesService = inject(InvoicesService);
  private readonly notify = inject(NotifyService);

  protected readonly methods = PAYMENT_METHODS;
  protected readonly methodLabel = METHOD_LABEL;
  protected readonly invoices = signal<Invoice[]>([]);
  protected readonly method = signal<PaymentMethod>('Card');
  protected readonly payingId = signal<string | null>(null);

  constructor() {
    this.load();
  }

  protected load(): void {
    this.invoicesService.mine().subscribe({ next: invoices => this.invoices.set(invoices), error: error => this.notify.error(error) });
  }

  protected pay(invoice: Invoice): void {
    this.payingId.set(invoice.id);
    this.invoicesService.pay(invoice.id, this.method()).subscribe({
      next: payment => {
        this.notify.success(`Invoice ${payment.invoiceNumber} paid (${this.methodLabel[payment.method]}).`);
        this.payingId.set(null);
        this.load();
      },
      error: error => { this.notify.error(error); this.payingId.set(null); }
    });
  }
}
