import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { DashboardService, DashboardStats, RevenueDataPoint, OccupancyReport } from '../../services/dashboard.service';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit, OnDestroy {
  currentDate = new Date();
  isAdmin = false;
  stats: DashboardStats | null = null;
  loading = true;

  // Occupancy properties
  occupancyReport: OccupancyReport | null = null;

  // Revenue Chart properties
  revenueMode: 'Day' | 'Month' | 'Year' = 'Month';
  revenueData: RevenueDataPoint[] = [];
  chart: any = null;
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
    this.isAdmin = this.authService.getUser().role === 'Admin';
    if (this.isAdmin) {
      this.loadDashboardData();
      this.loadRevenueReport();
      this.loadOccupancyReport();
    }
  }

  loadDashboardData() {
    this.loading = true;
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
        // Wait for next tick to ensure canvas is in DOM
        setTimeout(() => this.initChart(), 0);
      },
      error: (err) => {
        console.error('Error loading revenue report:', err);
        this.loadingChart = false;
      }
    });
  }

  setRevenueMode(mode: 'Day' | 'Month' | 'Year') {
    if (this.revenueMode === mode) return;
    this.revenueMode = mode;
    this.loadRevenueReport();
  }

  applyFilter() {
    this.loadRevenueReport();
  }

  initChart() {
    const ctx = document.getElementById('revenueChart') as HTMLCanvasElement;
    if (!ctx) return;

    if (this.chart) {
      this.chart.destroy();
    }

    const labels = this.revenueData.map(d => d.label);
    const data = this.revenueData.map(d => d.revenue);

    this.chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: labels,
        datasets: [{
          label: 'Doanh thu (VND)',
          data: data,
          backgroundColor: 'rgba(99, 102, 241, 0.4)',
          borderColor: 'rgb(99, 102, 241)',
          borderWidth: 2,
          borderRadius: 8,
          hoverBackgroundColor: 'rgba(99, 102, 241, 0.6)',
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          tooltip: {
             callbacks: {
               label: (context) => {
                 const value = context.parsed.y ?? 0;
                 return ' Doanh thu: ' + new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
               }
             }
          }
        },
        scales: {
          y: {
            beginAtZero: true,
            grid: { display: true, color: 'rgba(0,0,0,0.05)' },
            ticks: {
              callback: (value) => {
                if (Number(value) >= 1000000) return (Number(value) / 1000000) + 'M';
                if (Number(value) >= 1000) return (Number(value) / 1000) + 'K';
                return value;
              }
            }
          },
          x: {
            grid: { display: false }
          }
        }
      }
    });
  }

  ngOnDestroy() {
    if (this.chart) {
      this.chart.destroy();
    }
  }
}

