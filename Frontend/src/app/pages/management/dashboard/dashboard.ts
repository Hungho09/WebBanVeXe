import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { interval, Subscription } from 'rxjs';
import { AuthService } from '../../../services/auth.service';
import { DashboardService, DashboardStats, RevenueDataPoint, OccupancyReport } from '../../../services/dashboard.service';

// Import Child Components
import { DashboardHeroComponent } from './components/hero/hero.component';
import { DashboardStatsComponent } from './components/stats/stats.component';
import { DashboardRevenueChartComponent } from './components/revenue-chart/revenue-chart.component';
import { DashboardOccupancyComponent } from './components/occupancy/occupancy.component';
import { DashboardRecentBookingsComponent } from './components/recent-bookings/recent-bookings.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule,
    DashboardHeroComponent,
    DashboardStatsComponent,
    DashboardRevenueChartComponent,
    DashboardOccupancyComponent,
    DashboardRecentBookingsComponent
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit, OnDestroy {
  // Subscription handles periodic background data updates (Auto-refresh)
  // Quản lý việc tự động cập nhật dữ liệu ngầm theo chu kỳ (Làm mới tự động)
  private pollSubscription?: Subscription;
  currentDate = new Date();
  userName = 'Admin';
  isAdmin = false;
  stats: DashboardStats | null = null;
  loading = true;

  // Occupancy properties
  occupancyReport: OccupancyReport | null = null;

  // Revenue Chart properties
  revenueMode: 'Day' | 'Month' | 'Year' = 'Month';
  revenueData: RevenueDataPoint[] = [];
  loadingChart = false;

  // Filter properties
  filterStartDate: string = '';
  filterEndDate: string = '';
  filterYear: number = new Date().getFullYear();
  filterYearStart: number = new Date().getFullYear() - 5;
  filterYearEnd: number = new Date().getFullYear();
  availableYears: number[] = [];

  constructor(
    private authService: AuthService,
    private dashboardService: DashboardService
  ) {
    const currentYear = new Date().getFullYear();
    for (let i = currentYear; i >= currentYear - 10; i--) {
      this.availableYears.push(i);
    }
  }

  ngOnInit() {
    const user = this.authService.getUser();
    this.isAdmin = user.role === 'Admin';
    this.userName = user.fullName || 'Admin';
    
    if (this.isAdmin) {
      this.loadAllData();
      
      /** 
       * Real-time Simulation: Refresh dashboard data every 10 seconds 
       * Mô phỏng thời gian thực: Tự động làm mới dữ liệu Dashboard mỗi 10 giây 
       */
      this.pollSubscription = interval(10000).subscribe(() => {
        // Fetch data silently (without full page loading indicator)
        // Lấy dữ liệu ngầm (không hiện vòng xoay loading toàn trang)
        this.loadAllData(false); 
      });
    }
  }

  ngOnDestroy() {
    // Unsubscribe to prevent memory leaks when leaving the component
    // Hủy đăng ký để tránh rò rỉ bộ nhớ khi rời khỏi trang Dashboard
    if (this.pollSubscription) {
      this.pollSubscription.unsubscribe();
    }
  }

  loadAllData(showLoading = true) {
    this.loadDashboardData(showLoading);
    this.loadRevenueReport();
    this.loadOccupancyReport();
  }

  loadDashboardData(showLoading = true) {
    if (showLoading) this.loading = true;
    this.dashboardService.getStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading dashboard stats:', err);
        this.loading = false;
      }
    });
  }

  loadOccupancyReport() {
    this.dashboardService.getOccupancyReport().subscribe({
      next: (res) => {
        this.occupancyReport = res;
      },
      error: (err) => {
        console.error('Error loading occupancy report:', err);
      }
    });
  }

  loadRevenueReport() {
    this.loadingChart = true;
    let start: string | undefined;
    let end: string | undefined;

    if (this.revenueMode === 'Day') {
      start = this.filterStartDate || undefined;
      end = this.filterEndDate || undefined;
    } else if (this.revenueMode === 'Month') {
      start = `${this.filterYear}-01-01`;
      end = `${this.filterYear}-12-31`;
    } else if (this.revenueMode === 'Year') {
      start = `${this.filterYearStart}-01-01`;
      end = `${this.filterYearEnd}-12-31`;
    }

    this.dashboardService.getRevenueReport(this.revenueMode, start, end).subscribe({
      next: (res) => {
        this.revenueData = res.revenueDetails;
        this.loadingChart = false;
      },
      error: (err) => {
        console.error('Error loading revenue report:', err);
        this.loadingChart = false;
      }
    });
  }

  onModeChanged(mode: 'Day' | 'Month' | 'Year') {
    this.revenueMode = mode;
    this.loadRevenueReport();
  }

  onFilterChanged(filters: any) {
    this.filterStartDate = filters.startDate;
    this.filterEndDate = filters.endDate;
    this.filterYear = filters.year;
    this.filterYearStart = filters.yearStart;
    this.filterYearEnd = filters.yearEnd;
    this.loadRevenueReport();
  }
}

