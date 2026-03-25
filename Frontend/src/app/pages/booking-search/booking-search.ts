import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Trip, TripService } from '../../services/trip.service';

@Component({
  selector: 'app-booking-search',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './booking-search.html',
  styleUrl: './booking-search.css'
})
export class BookingSearch implements OnInit {
  origin: string = '';
  destination: string = '';
  date: string = '';
  
  trips: Trip[] = [];
  filteredTrips: Trip[] = [];
  isLoading = true;
  
  sortBy: string = 'default';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tripService: TripService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.origin = params['origin'] || '';
      this.destination = params['destination'] || '';
      this.date = params['date'] || '';
      this.loadTrips();
    });
  }

  loadTrips() {
    this.isLoading = true;
    this.tripService.getTrips().subscribe({
      next: (data) => {
        this.trips = data.filter(t => t.status === 'Active');
        this.applyFilters();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  applyFilters() {
    let result = [...this.trips];
    
    if (this.origin) {
      result = result.filter(t => t.routeName.toLowerCase().includes(this.origin.toLowerCase()));
    }
    
    if (this.destination) {
      result = result.filter(t => t.routeName.toLowerCase().includes(this.destination.toLowerCase()));
    }
    
    // Simple date filter (checking if departure date matches)
    if (this.date) {
      const searchDate = new Date(this.date).toDateString();
      result = result.filter(t => new Date(t.departureTime).toDateString() === searchDate);
    }

    this.sortTrips(result);
  }

  onSortChange(type: string) {
    this.sortBy = type;
    this.sortTrips(this.filteredTrips);
  }

  sortTrips(list: Trip[]) {
    switch (this.sortBy) {
      case 'early':
        list.sort((a, b) => new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime());
        break;
      case 'late':
        list.sort((a, b) => new Date(b.departureTime).getTime() - new Date(a.departureTime).getTime());
        break;
      case 'priceAsc':
        list.sort((a, b) => a.price - b.price);
        break;
      case 'priceDesc':
        list.sort((a, b) => b.price - a.price);
        break;
      default:
        // Default might be by creation time or original order
        break;
    }
    this.filteredTrips = list;
  }

  formatTime(dateStr: string): string {
    const d = new Date(dateStr);
    return d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
  }

  formatDate(dateStr: string): string {
    const d = new Date(dateStr);
    return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }

  getDuration(start: string, end: string): string {
    const s = new Date(start);
    const e = new Date(end);
    const diff = e.getTime() - s.getTime();
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    return `${hours}h ${minutes}m`;
  }
}
