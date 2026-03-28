import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RecentBooking {
  id: string;
  customerName: string;
  customerAvatar: string;
  routeName: string;
  departureTime: string;
  busPlate: string;
  status: string;
}

export interface DashboardStats {
  totalBookings: number;
  totalRevenue: number;
  totalUsers: number;
  activeTrips: number;
  mostPopularRoute: string;
  recentBookings: RecentBooking[];
}

export interface RevenueDataPoint {
  date: string;
  label: string;
  revenue: number;
  bookingCount: number;
}

export interface RevenueReport {
  totalRevenue: number;
  totalPaidBookings: number;
  revenueDetails: RevenueDataPoint[];
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

  getRevenueReport(mode: string = 'Day', start?: string, end?: string): Observable<RevenueReport> {
    let url = `${this.apiUrl}/revenue?mode=${mode}`;
    if (start) url += `&startDate=${start}`;
    if (end) url += `&endDate=${end}`;
    return this.http.get<RevenueReport>(url);
  }
}
