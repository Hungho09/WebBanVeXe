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
    // Return mock data for now to ensure the page works without backend running
    return of([
      {
        id: '1',
        routeId: 'r1',
        routeName: 'Hà Nội - Hải Phòng',
        busId: 'b1',
        busPlate: '29A-12345',
        departureTime: '2026-03-20T08:00:00',
        arrivalTime: '2026-03-20T10:00:00',
        price: 150000,
        status: 'Active'
      },
      {
        id: '2',
        routeId: 'r2',
        routeName: 'Sài Gòn - Đà Lạt',
        busId: 'b2',
        busPlate: '51B-67890',
        departureTime: '2026-03-21T22:00:00',
        arrivalTime: '2026-03-22T06:00:00',
        price: 350000,
        status: 'Active'
      }
    ]);
    // Once integrated: return this.http.get<Trip[]>(this.apiUrl);
  }

  getTrip(id: string): Observable<Trip> {
    return this.http.get<Trip>(`${this.apiUrl}/${id}`);
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
}
