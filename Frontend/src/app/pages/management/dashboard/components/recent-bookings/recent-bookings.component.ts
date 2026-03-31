import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardStats } from '../../../../../services/dashboard.service';

@Component({
  selector: 'app-dashboard-recent-bookings',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recent-bookings.component.html',
  styleUrl: './recent-bookings.component.css'
})
export class DashboardRecentBookingsComponent {
  @Input() stats: DashboardStats | null = null;
}
