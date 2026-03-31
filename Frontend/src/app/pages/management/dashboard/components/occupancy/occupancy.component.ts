import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OccupancyReport } from '../../../../../services/dashboard.service';

@Component({
  selector: 'app-dashboard-occupancy',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './occupancy.component.html',
  styleUrl: './occupancy.component.css'
})
export class DashboardOccupancyComponent {
  @Input() occupancyReport: OccupancyReport | null = null;
}
