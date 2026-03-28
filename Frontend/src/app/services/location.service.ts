import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LocationModel {
  id: string;
  name: string;
  address: string;
  provinceName?: string;
  mapLink?: string;
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
}
