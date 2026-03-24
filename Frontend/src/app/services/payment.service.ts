import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreatePaymentDto, PaymentResultDto } from '../models/payment.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private apiUrl = '/api/payment';

  constructor(private http: HttpClient) { }

  processPayment(payment: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, payment);
  }

  getPaymentById(id: string): Observable<PaymentResultDto> {
    return this.http.get<PaymentResultDto>(`${this.apiUrl}/${id}`);
  }

  getPaymentsByBookingId(bookingId: string): Observable<PaymentResultDto[]> {
    return this.http.get<PaymentResultDto[]>(`${this.apiUrl}/booking/${bookingId}`);
  }
}
