import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice } from '../models/invoice.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private readonly apiUrl = '/api/invoices';

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(this.apiUrl);
  }

  getInvoiceById(id: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.apiUrl}/${id}`);
  }

  getInvoices(): Observable<Invoice[]> {
    return this.getAll();
  }

  getByBookingId(bookingId: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.apiUrl}/booking/${bookingId}`);
  }

  createInvoice(bookingId: string): Observable<Invoice> {
    return this.http.post<Invoice>(`${this.apiUrl}/create/${bookingId}`, {});
  }
}
