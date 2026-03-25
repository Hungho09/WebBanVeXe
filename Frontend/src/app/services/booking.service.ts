import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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
  totalAmount: number;
  bookingStatus: string;
  createdAt: string;
  details: BookingDetailDto[];
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private apiUrl = 'api/bookings';

  constructor(private http: HttpClient) {}

  createBooking(dto: CreateBookingDto): Observable<BookingResponseDto> {
    return this.http.post<BookingResponseDto>(this.apiUrl, dto);
  }

  getBooking(id: string): Observable<BookingResponseDto> {
    return this.http.get<BookingResponseDto>(`${this.apiUrl}/${id}`);
  }

  getUserBookings(userId: string): Observable<BookingResponseDto[]> {
    return this.http.get<BookingResponseDto[]>(`${this.apiUrl}/user/${userId}`);
  }

  cancelBooking(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  lockSeat(seatId: string, userId: string): Observable<any> {
    return this.http.post(`api/bookings/lock-seat/${seatId}`, { userId });
  }

  unlockSeat(seatId: string, userId: string): Observable<any> {
    return this.http.post(`api/bookings/unlock-seat/${seatId}`, { userId });
  }
}
