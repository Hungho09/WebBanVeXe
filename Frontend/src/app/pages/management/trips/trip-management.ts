import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Trip, TripService } from '../../../services/trip.service';
import { Route, RouteService } from '../../../services/route.service';
import { Bus, BusService } from '../../../services/bus.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-trip-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './trip-management.html',
  styleUrls: ['./trip-management.css']
})
export class TripManagement implements OnInit {
  private readonly toastService = inject(ToastService);
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
    this.tripService.getTrips().subscribe({
      next: (trips) => {
        this.tripList = trips;
        this.onSearch();
      },
      error: () => this.toastService.showError('Không thể tải danh sách chuyến đi')
    });
  }

  loadRoutes() {
    this.routeService.getRoutes().subscribe({
      next: (routes) => this.routes = routes.filter(r => r.isActive),
      error: () => this.toastService.showError('Không thể tải danh sách tuyến đường')
    });
  }

  loadBuses() {
    this.busService.getBuses().subscribe({
      next: (buses) => {
        this.availableBuses = buses.filter(b => b.status === "Available" as any);
      },
      error: () => this.toastService.showError('Không thể tải danh sách xe')
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
      
      if (this.isEditMode && this.currentTripId) {
        this.tripService.updateTrip(this.currentTripId, tripData).subscribe({
          next: () => {
            this.toastService.showSuccess('Cập nhật chuyến đi thành công');
            this.loadTrips();
            this.closeModal();
          },
          error: (err) => {
            const msg = typeof err.error === 'string' ? err.error : (err.error?.message || err.message);
            this.toastService.showError('Lỗi cập nhật: ' + msg);
          }
        });
      } else {
        this.tripService.createTrip(tripData).subscribe({
          next: () => {
            this.toastService.showSuccess('Thêm chuyến đi mới thành công');
            this.loadTrips();
            this.loadBuses();
            this.closeModal();
          },
          error: (err) => {
            const msg = typeof err.error === 'string' ? err.error : (err.error?.message || err.message);
            this.toastService.showError('Lỗi tạo mới: ' + msg);
          }
        });
      }
    }
  }

  onDelete(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa chuyến đi này?')) {
      this.tripService.deleteTrip(id).subscribe({
        next: () => {
          this.toastService.showSuccess('Đã xóa chuyến đi');
          this.loadTrips();
        },
        error: (err) => this.toastService.showError('Lỗi khi xóa: ' + (err.error?.message || err.message))
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
