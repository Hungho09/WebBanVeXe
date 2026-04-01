import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice } from '../models/invoice.model';

export interface CreateBookingDto {
  userId: string;
  tripId: string;
  seatIds: string[];
  pickupPointId?: string;
  dropoffPointId?: string;
}

export interface BookingDetailDto {
  id: string;
  seatId: string;
  seatNumber: string;
  price: number;
}

export interface BookingResponseDto {
  id: string;
  userId: string;
  userName: string;
  tripId: string;
  routeName: string;
  departureTime?: string;
  arrivalTime?: string;
  totalAmount: number;
  bookingStatus: string;
  createdAt: string;
  details: BookingDetailDto[];
  invoice?: Invoice;
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private apiUrl = '/api/bookings';

  constructor(private http: HttpClient) {}

  getAllBookings(): Observable<BookingResponseDto[]> {
    return this.http.get<BookingResponseDto[]>(this.apiUrl);
  }

  approveCancel(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/approve-cancel`, {});
  }

  createBooking(dto: CreateBookingDto): Observable<BookingResponseDto> {
    return this.http.post<BookingResponseDto>(this.apiUrl, dto);
  }

  getBooking(id: string): Observable<BookingResponseDto> {
    return this.http.get<BookingResponseDto>(`${this.apiUrl}/${id}`);
  }

  getUserBookings(userId: string): Observable<BookingResponseDto[]> {
    return this.http.get<BookingResponseDto[]>(`${this.apiUrl}/user/${userId}`);
  }

  getCancelRequests(): Observable<BookingResponseDto[]> {
    return this.http.get<BookingResponseDto[]>(`${this.apiUrl}/cancel-requests`);
  }

  cancelBooking(id: string, userId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/cancel-request`, { userId });
  }

  approveCancelBooking(id: string, adminUserId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/approve-cancel`, { adminUserId });
  }

  confirmPayment(id: string, adminUserId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/confirm-payment`, { adminUserId });
  }

  lockSeat(seatId: string, userId: string): Observable<any> {
    return this.http.post(`/api/trip/seats/${seatId}/lock`, { userId });
  }

  unlockSeat(seatId: string, userId: string): Observable<any> {
    return this.http.post(`/api/trip/seats/${seatId}/unlock`, { userId });
  }

  processPayment(bookingId: string, paymentMethod: string): Observable<any> {
    return this.http.post(`/api/payment`, { bookingId, paymentMethod });
  }
}
