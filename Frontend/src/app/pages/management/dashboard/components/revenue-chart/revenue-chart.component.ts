import { Component, Input, OnInit, OnDestroy, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chart, registerables } from 'chart.js';
import { RevenueDataPoint } from '../../../../../services/dashboard.service';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard-revenue-chart',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './revenue-chart.component.html',
  styleUrl: './revenue-chart.component.css'
})
export class DashboardRevenueChartComponent implements OnInit, OnDestroy, OnChanges {
  @Input() revenueData: RevenueDataPoint[] = [];
  @Input() loadingChart = false;
  @Input() revenueMode: 'Day' | 'Month' | 'Year' = 'Month';
  @Input() availableYears: number[] = [];
  
  @Input() filterStartDate: string = '';
  @Input() filterEndDate: string = '';
  @Input() filterYear: number = new Date().getFullYear();
  @Input() filterYearStart: number = new Date().getFullYear() - 5;
  @Input() filterYearEnd: number = new Date().getFullYear();

  @Output() filterChanged = new EventEmitter<any>();
  @Output() modeChanged = new EventEmitter<'Day' | 'Month' | 'Year'>();

  chart: any = null;

  ngOnInit() {
    // Initial chart will be handled by ngOnChanges when data arrives
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['revenueData'] && this.revenueData.length > 0) {
      setTimeout(() => this.initChart(), 0);
    }
  }

  setRevenueMode(mode: 'Day' | 'Month' | 'Year') {
    this.modeChanged.emit(mode);
  }

  applyFilter() {
    this.filterChanged.emit({
      startDate: this.filterStartDate,
      endDate: this.filterEndDate,
      year: this.filterYear,
      yearStart: this.filterYearStart,
      yearEnd: this.filterYearEnd
    });
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
               label: (context: any) => {
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
              callback: (value: any) => {
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
