import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Bus {
  id: string;
  plateNumber: string;
  busType: string;
  seatCapacity: number;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class BusService {
  private apiUrl = 'api/buses';

  constructor(private http: HttpClient) {}

  getBuses(): Observable<Bus[]> {
    return this.http.get<Bus[]>(this.apiUrl);
  }

  getBus(id: string): Observable<Bus> {
    return this.http.get<Bus>(`${this.apiUrl}/${id}`);
  }

  createBus(bus: any): Observable<Bus> {
    return this.http.post<Bus>(this.apiUrl, bus);
  }

  updateBus(id: string, bus: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, bus);
  }

  deleteBus(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
