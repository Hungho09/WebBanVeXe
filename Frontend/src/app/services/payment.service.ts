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

  createPayment(payment: CreatePaymentDto): Observable<PaymentResultDto> {
    return this.http.post<PaymentResultDto>(this.apiUrl, payment);
  }

  processPayment(id: string): Observable<PaymentResultDto> {
    return this.http.post<PaymentResultDto>(`${this.apiUrl}/${id}/process`, {});
  }

  getPaymentById(id: string): Observable<PaymentResultDto> {
    return this.http.get<PaymentResultDto>(`${this.apiUrl}/${id}`);
  }

  getPaymentsByBookingId(bookingId: string): Observable<PaymentResultDto[]> {
    return this.http.get<PaymentResultDto[]>(`${this.apiUrl}/booking/${bookingId}`);
  }
}
