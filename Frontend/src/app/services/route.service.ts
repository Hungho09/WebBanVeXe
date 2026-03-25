import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Route {
  id: string;
  origin: string;
  destination: string;
  distanceKm: number;
  points?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class RouteService {
  private apiUrl = '/api/routes';

  constructor(private http: HttpClient) {}

  getRoutes(): Observable<Route[]> {
    return this.http.get<Route[]>(this.apiUrl);
  }

  getRoute(id: string): Observable<Route> {
    return this.http.get<Route>(`${this.apiUrl}/${id}`);
  }

  createRoute(route: any): Observable<Route> {
    return this.http.post<Route>(this.apiUrl, route);
  }

  updateRoute(id: string, route: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, route);
  }

  deleteRoute(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getLocations(): Observable<{ origins: string[], destinations: string[] }> {
    return this.http.get<{ origins: string[], destinations: string[] }>(`${this.apiUrl}/locations`);
  }
}
