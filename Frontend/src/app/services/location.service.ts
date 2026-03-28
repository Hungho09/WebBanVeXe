import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Province {
  id: string;
  name: string;
  slug: string;
  region: string;
}

export interface LocationModel {
  id: string;
  name: string;
  address: string;
  latitude?: number;
  longitude?: number;
  isPickup: boolean;
  isDropoff: boolean;
  badge?: string;
  provinceId?: string;
  province?: Province;
  mapLink?: string;
  isDefault: boolean;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private apiUrl = '/api/v1/Locations';

  constructor(private http: HttpClient) {}

  getLocations(searchTerm?: string): Observable<LocationModel[]> {
    let params = new HttpParams();
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    return this.http.get<LocationModel[]>(this.apiUrl, { params });
  }

  getProvinces(): Observable<Province[]> {
    return this.http.get<Province[]>(`${this.apiUrl}/provinces`);
  }

  getLocationById(id: string): Observable<LocationModel> {
    return this.http.get<LocationModel>(`${this.apiUrl}/${id}`);
  }

  createLocation(data: Partial<LocationModel>): Observable<LocationModel> {
    return this.http.post<LocationModel>(this.apiUrl, data);
  }

  updateLocation(id: string, data: Partial<LocationModel>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }

  deleteLocation(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  toggleDefault(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/toggle-default`, {});
  }
}
