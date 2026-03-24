import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Bus, BusService, BusType } from '../../services/bus.service';

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
  selectedFile: File | null = null;
  imagePreview: string | null = null;

  busTypes: BusType[] = [];

  /** Status label map */
  statusLabels: Record<number, string> = {
    1: 'Đang hoạt động',
    2: 'Có sẵn',
    3: 'Ngưng hoạt động'
  };

  statusBadgeClass: Record<number, string> = {
    1: 'status-badge active',
    2: 'status-badge available',
    3: 'status-badge inactive'
  };

  constructor(
    private fb: FormBuilder, 
    private busService: BusService,
    private router: Router
  ) {
    this.busForm = this.fb.group({
      licensePlate: ['', [Validators.required, Validators.minLength(3)]],
      companyName: ['', Validators.required],
      busTypeId: ['', Validators.required],
      seatCount: [{ value: 0, disabled: true }],
      imageUrl: [''],
      status: [2]
    });
  }

  ngOnInit() {
    this.loadBusTypes();
    this.loadBuses();

    // Auto-update seatCount when busTypeId changes
    this.busForm.get('busTypeId')?.valueChanges.subscribe((typeId: string) => {
      const type = this.busTypes.find(t => t.id === typeId);
      if (type) {
        this.busForm.patchValue({ seatCount: type.seatCount }, { emitEvent: false });
      }
    });
  }

  loadBusTypes() {
    this.busService.getBusTypes().subscribe({
      next: (types) => this.busTypes = types,
      error: (err) => console.error('Error loading bus types', err)
    });
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
      item.licensePlate.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      item.companyName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      item.busType.name.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  onBusTypeChange(e: any) {
    const typeId = e.target.value;
    const type = this.busTypes.find(t => t.id === typeId);
    if (type) {
      this.busForm.patchValue({ seatCount: type.seatCount });
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = (e: any) => this.imagePreview = e.target.result;
      reader.readAsDataURL(file);
    }
  }

  getStatusLabel(status: number): string {
    return this.statusLabels[status] ?? 'Không xác định';
  }

  getStatusClass(status: number): string {
    return this.statusBadgeClass[status] ?? 'status-badge';
  }

  openAddModal() {
    this.isEditMode = false;
    this.currentBusId = null;
    this.imagePreview = null;
    this.selectedFile = null;
    this.busForm.reset({ status: 2 });
    this.isModalOpen = true;
  }

  openEditModal(bus: Bus) {
    this.isEditMode = true;
    this.currentBusId = bus.id;
    this.imagePreview = bus.imageUrl || null;
    this.selectedFile = null;
    this.busForm.patchValue({
      licensePlate: bus.licensePlate,
      companyName: bus.companyName,
      busTypeId: bus.busType.id,
      seatCount: bus.seatCount,
      imageUrl: bus.imageUrl,
      status: bus.status ?? 2
    });
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  saveBus() {
    if (this.busForm.invalid) return;
    if (this.selectedFile) {
      this.busService.uploadImage(this.selectedFile).subscribe({
        next: (res) => {
          this.busForm.patchValue({ imageUrl: res.url });
          this.executeSave();
        },
        error: (err) => alert('Lỗi tải ảnh: ' + (err.error?.message || err.message))
      });
    } else {
      this.executeSave();
    }
  }

  private executeSave() {
    const raw = this.busForm.getRawValue();
    const busData = {
      licensePlate: raw.licensePlate,
      companyName: raw.companyName,
      busTypeId: raw.busTypeId,
      imageUrl: raw.imageUrl || null,
      status: Number(raw.status),
      isActive: Number(raw.status) !== 3
    };
    
    if (this.isEditMode && this.currentBusId) {
      this.busService.updateBus(this.currentBusId, { ...busData, id: this.currentBusId }).subscribe({
        next: () => { this.loadBuses(); this.closeModal(); },
        error: (err) => alert('Lỗi: ' + (err.error?.message || err.message))
      });
    } else {
      this.busService.createBus(busData).subscribe({
        next: () => { this.loadBuses(); this.closeModal(); },
        error: (err) => alert('Lỗi: ' + (err.error?.message || err.message))
      });
    }
  }

  onDelete(id: string) {
    if (confirm('Xóa xe này?')) {
      this.busService.deleteBus(id).subscribe({
        next: () => this.loadBuses(),
        error: (err) => alert('Lỗi: ' + (err.error?.message || err.message))
      });
    }
  }
}

