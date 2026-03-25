import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardStats {
  totalBookings: number;
  totalRevenue: number;
  totalUsers: number;
  activeTrips: number;
  mostPopularRoute: string;
}

export interface RevenueReport {
  totalRevenue: number;
  totalPaidBookings: number;
  dailyRevenue: Array<{
    date: string;
    revenue: number;
    bookingCount: number;
  }>;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = '/api/dashboard';

  constructor(private http: HttpClient) { }

  getStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.apiUrl}/stats`);
  }

  getRevenueReport(start?: string, end?: string): Observable<RevenueReport> {
    let url = `${this.apiUrl}/revenue`;
    if (start && end) {
      url += `?start=${start}&end=${end}`;
    }
    return this.http.get<RevenueReport>(url);
  }
}
