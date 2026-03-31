import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Trip, TripService } from '../../../services/trip.service';

// Sub-components
import { SearchFilterSidebarComponent } from './components/search-filter-sidebar/search-filter-sidebar';
import { SearchResultCardComponent } from './components/search-result-card/search-result-card';
import { SearchHeaderMiniComponent } from './components/search-header-mini/search-header-mini';

@Component({
  selector: 'app-booking-search',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    SearchFilterSidebarComponent,
    SearchResultCardComponent,
    SearchHeaderMiniComponent
  ],
  templateUrl: './booking-search.html',
  styleUrl: './booking-search.css'
})
export class BookingSearch implements OnInit {
  origin: string = '';
  destination: string = '';
  date: string = '';
  routeId: string = '';
  
  trips: Trip[] = [];
  filteredTrips: Trip[] = [];
  isLoading = true;
  
  sortBy: string = 'default';
  activeFilters: any = {};

  // For Sidebar
  distinctCompanies: string[] = [];

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
      this.routeId = params['routeId'] || '';
      this.loadTrips();
    });
  }

  loadTrips() {
    this.isLoading = true;
    
    // Convert YYYY-MM-DD back if needed, but our backend can handle standard ISO or short date
    this.tripService.searchTrips(this.origin, this.destination, this.date).subscribe({
      next: (data) => {
        this.trips = data.filter(t => t.status === 'Active');
        this.distinctCompanies = Array.from(new Set(this.trips.map(t => 'Nhà xe ' + t.busPlate.split('-')[0]))); // Demo logic
        this.applyFilters();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  onFilterChange(filters: any) {
    this.activeFilters = filters;
    this.applyFilters();
  }

  onSortChange(type: string) {
    this.sortBy = type;
    this.applyFilters();
  }

  onHeaderUpdate(payload: any) {
    this.router.navigate(['/search-results'], { queryParams: payload });
  }

  applyFilters() {
    let result = [...this.trips];
    
    // Core search filters
    if (this.routeId) {
       result = result.filter(t => t.routeId === this.routeId);
    }
    
    // Notice: Origin, Destination, and Date are now pre-filtered by the backend!

    // Sidebar filters
    if (this.activeFilters.companies && this.activeFilters.companies.length > 0) {
      result = result.filter(t => this.activeFilters.companies.some((c: string) => ('Nhà xe ' + t.busPlate.split('-')[0]).includes(c)));
    }

    if (this.activeFilters.maxPrice) {
      result = result.filter(t => t.price <= this.activeFilters.maxPrice);
    }

    this.sortTrips(result);
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
        // Original order
        break;
    }
    this.filteredTrips = list;
  }

  onSelectTrip(id: string) {
    this.router.navigate(['/booking', id]);
  }
}

