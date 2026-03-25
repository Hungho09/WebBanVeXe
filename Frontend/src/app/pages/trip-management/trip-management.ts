import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Trip, TripService } from '../../services/trip.service';
import { Route, RouteService } from '../../services/route.service';
import { Bus, BusService } from '../../services/bus.service';

@Component({
  selector: 'app-trip-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './trip-management.html',
  styleUrls: ['./trip-management.css']
})
export class TripManagement implements OnInit {
  tripList: Trip[] = [];
  filteredList: Trip[] = [];
  searchTerm: string = '';
  
  isModalOpen = false;
  tripForm: FormGroup;
  isEditMode = false;
  currentTripId: string | null = null;

  // Data for dropdowns
  routes: Route[] = [];
  availableBuses: Bus[] = [];

  constructor(
    private fb: FormBuilder, 
    private tripService: TripService,
    private routeService: RouteService,
    private busService: BusService
  ) {
    this.tripForm = this.fb.group({
      routeId: ['', Validators.required],
      busId: ['', Validators.required],
      departureTime: ['', Validators.required],
      arrivalTime: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      status: ['Active', Validators.required]
    });
  }

  ngOnInit() {
    this.loadTrips();
    this.loadRoutes();
    this.loadBuses();
  }

  loadTrips() {
    this.tripService.getTrips().subscribe(trips => {
      this.tripList = trips;
      this.onSearch();
    });
  }

  loadRoutes() {
    this.routeService.getRoutes().subscribe({
      next: (routes) => this.routes = routes.filter(r => r.isActive),
      error: (err) => console.error('Lỗi tải tuyến đường', err)
    });
  }

  loadBuses() {
    this.busService.getBuses().subscribe({
      next: (buses) => {
        // Only show Available buses (status=2) for new trip creation
        this.availableBuses = buses.filter(b => b.status === 2);
      },
      error: (err) => console.error('Lỗi tải danh sách xe', err)
    });
  }

  onSearch() {
    this.filteredList = this.tripList.filter(item => 
      item.routeName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      item.busPlate.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  openAddModal() {
    this.isEditMode = false;
    this.currentTripId = null;
    this.tripForm.reset({ status: 'Active', price: 0 });
    this.isModalOpen = true;
  }

  openEditModal(trip: Trip) {
    this.isEditMode = true;
    this.currentTripId = trip.id;
    // In edit mode, show current bus even if it's "Active" status
    if (!this.availableBuses.find(b => b.id === trip.busId)) {
      this.busService.getBus(trip.busId).subscribe(bus => {
        if (bus && !this.availableBuses.find(b => b.id === bus.id)) {
          this.availableBuses = [bus, ...this.availableBuses];
        }
      });
    }
    const dep = trip.departureTime ? trip.departureTime.substring(0, 16) : '';
    const arr = trip.arrivalTime ? trip.arrivalTime.substring(0, 16) : '';
    this.tripForm.patchValue({
      routeId: trip.routeId,
      busId: trip.busId,
      departureTime: dep,
      arrivalTime: arr,
      price: trip.price,
      status: trip.status
    });
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  saveTrip() {
    if (this.tripForm.valid) {
      const tripData = this.tripForm.value;
      console.log('Update Trip ID:', this.currentTripId);
      console.log('Form Data to Send:', tripData);
      
      if (this.isEditMode && this.currentTripId) {
        this.tripService.updateTrip(this.currentTripId, tripData).subscribe({
          next: () => {
            this.loadTrips();
            this.closeModal();
          },
          error: (err) => {
            console.error('Update Error Response:', err);
            const msg = typeof err.error === 'string' ? err.error : (err.error?.message || err.message);
            alert('Lỗi khi cập nhật: ' + msg);
          }
        });
      } else {
        console.log('Creating new trip...');
        this.tripService.createTrip(tripData).subscribe({
          next: () => {
            this.loadTrips();
            this.loadBuses(); // Refresh available buses
            this.closeModal();
          },
          error: (err) => {
            const msg = typeof err.error === 'string' ? err.error : (err.error?.message || err.message);
            alert('Lỗi khi tạo mới: ' + msg);
          }
        });
      }
    }
  }

  onDelete(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa chuyến đi này?')) {
      this.tripService.deleteTrip(id).subscribe({
        next: () => this.loadTrips(),
        error: (err) => alert('Lỗi khi xóa: ' + (err.error?.message || err.message))
      });
    }
  }

  getStatusClass(status: string) {
    switch (status) {
      case 'Active': return 'status-available';
      case 'Completed': return 'status-maintenance';
      case 'Cancelled': return 'status-out';
      default: return '';
    }
  }

  getRouteLabel(route: Route): string {
    return `${route.origin} → ${route.destination} (${route.distanceKm}km)`;
  }
}
