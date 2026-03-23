import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Trip, TripService } from '../../services/trip.service';

@Component({
  selector: 'app-trip-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule], // Thêm RouterLink
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

  constructor(
    private fb: FormBuilder, 
    private tripService: TripService
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
  }

  loadTrips() {
    this.tripService.getTrips().subscribe(trips => {
      this.tripList = trips;
      this.onSearch();
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
    this.tripForm.patchValue({
      routeId: trip.routeId,
      busId: trip.busId,
      departureTime: trip.departureTime,
      arrivalTime: trip.arrivalTime,
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
            this.loadTrips();
            this.closeModal();
          },
          error: (err) => alert('Lỗi khi cập nhật: ' + (err.error?.message || err.message))
        });
      } else {
        this.tripService.createTrip(tripData).subscribe({
          next: () => {
            this.loadTrips();
            this.closeModal();
          },
          error: (err) => alert('Lỗi khi tạo mới: ' + (err.error?.message || err.message))
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
}
