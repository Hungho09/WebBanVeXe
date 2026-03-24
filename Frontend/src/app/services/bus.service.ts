import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BusType {
  id: string;
  name: string;
  seatCount: number;
}

export type BusStatus = 1 | 2 | 3; // 1=Active, 2=Available, 3=Inactive

export interface Bus {
  id: string;
  licensePlate: string;
  companyName: string;
  imageUrl?: string;
  seatCount: number;
  busType: BusType;
  isActive: boolean;
  status: BusStatus;
  statusLabel: string;
}

@Injectable({
  providedIn: 'root'
})
export class BusService {
  private apiUrl = '/api/buses';

  constructor(private http: HttpClient) {}

  getBuses(): Observable<Bus[]> {
    return this.http.get<Bus[]>(this.apiUrl);
  }

  getBus(id: string): Observable<Bus> {
    return this.http.get<Bus>(`${this.apiUrl}/${id}`);
  }

  getAvailableBuses(): Observable<Bus[]> {
    return this.http.get<Bus[]>(`${this.apiUrl}?status=2`);
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

  getBusTypes(): Observable<BusType[]> {
    return this.http.get<BusType[]>('api/bustypes');
  }

  uploadImage(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>('api/upload', formData);
  }
}
