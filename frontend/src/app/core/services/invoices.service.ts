import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Invoice, PaymentMethod, PaymentResult } from '../api/models';

@Injectable({ providedIn: 'root' })
export class InvoicesService {
  private readonly http = inject(HttpClient);

  mine() { return this.http.get<Invoice[]>('/api/invoices'); }
  pay(invoiceId: string, method: PaymentMethod) { return this.http.post<PaymentResult>(`/api/invoices/${invoiceId}/pay`, { method }); }
}
