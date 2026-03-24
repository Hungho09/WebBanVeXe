import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

export interface Trip {
  id: string;
  routeId: string;
  routeName: string;
  busId: string;
  busPlate: string;
  departureTime: string;
  arrivalTime: string;
  price: number;
  status: string;
}

@Injectable({
  providedIn: 'root'
})
export class TripService {
  private apiUrl = 'api/trip'; // Assuming relative path or proxy

  constructor(private http: HttpClient) {}

  getTrips(): Observable<Trip[]> {
    return this.http.get<Trip[]>(this.apiUrl);
  }

  getTrip(id: string): Observable<Trip> {
    return this.http.get<Trip>(`${this.apiUrl}/${id}`);
  }

  getSeatsByTrip(tripId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${tripId}/seats`);
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

  getSeats(tripId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${tripId}/seats`);
  }

  getTripPoints(tripId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${tripId}/points`);
  }
}
