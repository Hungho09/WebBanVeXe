import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardStats } from '../../../../../services/dashboard.service';

@Component({
  selector: 'app-dashboard-stats',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stats.component.html',
  styleUrl: './stats.component.css'
})
export class DashboardStatsComponent {
  @Input() stats: DashboardStats | null = null;
  @Input() loading: boolean = false;
}
