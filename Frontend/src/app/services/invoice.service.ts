import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice } from '../models/invoice.model';

export interface CreateInvoiceRequest {
  bookingId: string;
}

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private readonly apiUrl = '/api/invoices';

  constructor(private readonly http: HttpClient) {}

  // GET /api/invoice
  getAll(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(this.apiUrl);
  }

  // GET /api/invoice/{id}
  getInvoiceById(id: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.apiUrl}/${id}`);
  }

  getInvoices(): Observable<Invoice[]> {
    return this.getAll();
  }

  // GET /api/invoice/booking/{bookingId}
  getByBookingId(bookingId: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.apiUrl}/booking/${bookingId}`);
  }

  // POST /api/invoice (with bookingId in body)
  createInvoice(request: CreateInvoiceRequest): Observable<Invoice> {
    return this.http.post<Invoice>(this.apiUrl, request);
  }

  // POST /api/invoice/create/{bookingId} (alternative endpoint)
  createInvoiceByBookingId(bookingId: string): Observable<Invoice> {
    return this.http.post<Invoice>(`${this.apiUrl}/create/${bookingId}`, {});
  }
}
