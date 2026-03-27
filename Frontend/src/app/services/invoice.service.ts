import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { saveAs } from 'file-saver';
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

  // GET /api/invoice/{id}/export/json
  exportJson(id: string): Observable<void> {
    return new Observable(observer => {
      this.http.get(`${this.apiUrl}/${id}/export/json`, { 
        responseType: 'blob',
        observe: 'response' 
      }).subscribe({
        next: (response) => {
          const contentDisposition = response.headers.get('content-disposition');
          const filename = this.getFilenameFromContentDisposition(contentDisposition || 'invoice.json');
          
          saveAs(response.body!, filename);
          observer.next();
          observer.complete();
        },
        error: (err) => observer.error(err)
      });
    });
  }

  // GET /api/invoice/{id}/export/pdf
  exportPdf(id: string): Observable<void> {
    return new Observable(observer => {
      this.http.get(`${this.apiUrl}/${id}/export/pdf`, { 
        responseType: 'blob',
        observe: 'response' 
      }).subscribe({
        next: (response) => {
          const contentDisposition = response.headers.get('content-disposition');
          const filename = this.getFilenameFromContentDisposition(contentDisposition || 'invoice.pdf');
          
          saveAs(response.body!, filename);
          observer.next();
          observer.complete();
        },
        error: (err) => observer.error(err)
      });
    });
  }

  private getFilenameFromContentDisposition(contentDisposition: string): string {
    const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
    const matches = filenameRegex.exec(contentDisposition);
    
    if (matches && matches[1]) {
      return matches[1].replace(/['"]/g, '');
    }
    
    return 'invoice';
  }
}
