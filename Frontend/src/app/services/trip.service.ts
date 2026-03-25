import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Trip {
  id: string;
  routeId: string;
  routeName: string;
  busId: string;
  busPlate: string;
  busTypeName?: string;
  busImageUrl?: string;
  departureTime: string;
  arrivalTime: string;
  price: number;
  status: string;
  availableSeats?: number;
}

export interface Seat {
  id: string;
  tripId: string;
  seatNumber: string;
  status: string;
  rowNumber: number;
  columnNumber: number;
  floor: number;
}

@Injectable({
  providedIn: 'root'
})
export class TripService {
  private apiUrl = '/api/trip'; // Assuming relative path or proxy

  constructor(private http: HttpClient) {}

  getTrips(): Observable<Trip[]> {
    return this.http.get<Trip[]>(this.apiUrl);
  }

  searchTrips(origin: string, destination: string, date: string): Observable<Trip[]> {
    let params = new HttpParams();
    if (origin) params = params.set('origin', origin);
    if (destination) params = params.set('destination', destination);
    if (date) params = params.set('date', date);
    return this.http.get<Trip[]>(`${this.apiUrl}/search`, { params });
  }

  getTrip(id: string): Observable<Trip> {
    return this.http.get<Trip>(`${this.apiUrl}/${id}`);
  }

  getSeatsByTrip(tripId: string): Observable<Seat[]> {
    return this.http.get<Seat[]>(`${this.apiUrl}/${tripId}/seats`);
  }

  createTrip(trip: any): Observable<Trip> {
    return this.http.post<Trip>(this.apiUrl, trip);
  }

  updateTrip(id: string, trip: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, trip);
  }

  deleteTrip(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getTripPoints(tripId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${tripId}/points`);
  }
}
