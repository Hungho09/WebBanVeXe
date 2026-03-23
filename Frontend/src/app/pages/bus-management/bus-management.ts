import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Bus, BusService } from '../../services/bus.service';

@Component({
  selector: 'app-bus-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './bus-management.html',
  styleUrls: ['./bus-management.css']
})
export class BusManagement implements OnInit {
  busList: Bus[] = [];
  filteredList: Bus[] = [];
  searchTerm: string = '';
  
  isModalOpen = false;
  busForm: FormGroup;
  isEditMode = false;
  currentBusId: string | null = null;

  busTypes = ['Sleeper', 'Seat', 'Limousine'];

  constructor(
    private fb: FormBuilder, 
    private busService: BusService,
    private router: Router
  ) {
    this.busForm = this.fb.group({
      plateNumber: ['', [Validators.required, Validators.pattern(/^[0-9]{2}[A-Z]-[0-9]{4,5}$/)]],
      busType: ['Sleeper', Validators.required],
      seatCapacity: [36, [Validators.required, Validators.min(1)]],
      isActive: [true]
    });
  }

  ngOnInit() {
    this.loadBuses();
  }

  loadBuses() {
    this.busService.getBuses().subscribe({
      next: (buses) => {
        this.busList = buses;
        this.onSearch();
      },
      error: (err) => console.error('Error loading buses', err)
    });
  }

  onSearch() {
    this.filteredList = this.busList.filter(item => 
      item.plateNumber.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      item.busType.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  openAddModal() {
    this.isEditMode = false;
    this.currentBusId = null;
    this.busForm.reset({ busType: 'Sleeper', seatCapacity: 36, isActive: true });
    this.isModalOpen = true;
  }

  openEditModal(bus: Bus) {
    this.isEditMode = true;
    this.currentBusId = bus.id;
    this.busForm.patchValue({
      plateNumber: bus.plateNumber,
      busType: bus.busType,
      seatCapacity: bus.seatCapacity,
      isActive: bus.isActive
    });
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  saveBus() {
    if (this.busForm.valid) {
      const busData = this.busForm.value;
      
      if (this.isEditMode && this.currentBusId) {
        this.busService.updateBus(this.currentBusId, { ...busData, id: this.currentBusId }).subscribe({
          next: () => {
            this.loadBuses();
            this.closeModal();
          },
          error: (err) => alert('Lỗi khi cập nhật: ' + (err.error?.message || err.message))
        });
      } else {
        this.busService.createBus(busData).subscribe({
          next: () => {
            this.loadBuses();
            this.closeModal();
          },
          error: (err) => alert('Lỗi khi tạo mới: ' + (err.error?.message || err.message))
        });
      }
    }
  }

  onDelete(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa xe này?')) {
      this.busService.deleteBus(id).subscribe({
        next: () => this.loadBuses(),
        error: (err) => alert('Lỗi khi xóa: ' + (err.error?.message || err.message))
      });
    }
  }

  navigateTo(url: string) {
    this.router.navigateByUrl(url);
  }
}
