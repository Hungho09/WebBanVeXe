import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouteService } from '../../../../services/route.service';

@Component({
  selector: 'app-search-header-mini',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-header-mini.html',
  styleUrl: './search-header-mini.css'
})
export class SearchHeaderMiniComponent implements OnInit {
  @Input() origin: string = '';
  @Input() destination: string = '';
  @Input() date: string = '';
  
  @Output() searchChange = new EventEmitter<any>();

  dbOrigins: string[] = [];
  dbDestinations: string[] = [];
  showOriginDropdown: boolean = false;
  showDestinationDropdown: boolean = false;

  constructor(private routeService: RouteService) {}

  ngOnInit() {
    this.loadLocations();
  }

  loadLocations() {
    this.routeService.getLocations().subscribe({
      next: (res: { origins: string[], destinations: string[] }) => {
        this.dbOrigins = res.origins;
        this.dbDestinations = res.destinations;
      },
      error: (err: any) => console.error('Error loading locations:', err)
    });
  }

  get filteredOrigins(): string[] {
    return this.dbOrigins
      .filter(p => !this.origin || p.toLowerCase().includes(this.origin.toLowerCase()))
      .filter(p => p !== this.destination);
  }

  get filteredDestinations(): string[] {
    return this.dbDestinations
      .filter(p => !this.destination || p.toLowerCase().includes(this.destination.toLowerCase()))
      .filter(p => p !== this.origin);
  }

  selectOrigin(p: string) { this.origin = p; this.showOriginDropdown = false; }
  selectDestination(p: string) { this.destination = p; this.showDestinationDropdown = false; }
  onBlurOrigin() { setTimeout(() => this.showOriginDropdown = false, 200); }
  onBlurDestination() { setTimeout(() => this.showDestinationDropdown = false, 200); }

  onResubmit() {
    this.searchChange.emit({
      origin: this.origin,
      destination: this.destination,
      date: this.date
    });
  }
}
