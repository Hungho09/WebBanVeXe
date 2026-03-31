import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Bus, BusService, BusType } from '../../../services/bus.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-bus-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './bus-management.html',
  styleUrls: ['./bus-management.css']
})
export class BusManagement implements OnInit {
  private readonly toastService = inject(ToastService);
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
  statusLabels: Record<string, string> = {
    'Active': 'Đang hoạt động',
    'Available': 'Có sẵn',
    'Inactive': 'Ngưng hoạt động'
  };

  statusBadgeClass: Record<string, string> = {
    'Active': 'status-badge active',
    'Available': 'status-badge available',
    'Inactive': 'status-badge inactive'
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
      status: ['Available']
    });
  }

  ngOnInit() {
    this.loadBusTypes();
    this.loadBuses();

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
      error: () => this.toastService.showError('Không thể tải kiểu xe')
    });
  }

  loadBuses() {
    this.busService.getBuses().subscribe({
      next: (buses) => {
        this.busList = buses;
        this.onSearch();
      },
      error: () => this.toastService.showError('Không thể tải danh sách xe')
    });
  }

  onSearch() {
    this.filteredList = this.busList.filter(item => 
      item.licensePlate.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      item.companyName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      item.busType.name.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
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

  getStatusLabel(status: any): string {
    return this.statusLabels[status] ?? 'Không xác định';
  }

  getStatusClass(status: any): string {
    return this.statusBadgeClass[status] ?? 'status-badge';
  }

  openAddModal() {
    this.isEditMode = false;
    this.currentBusId = null;
    this.imagePreview = null;
    this.selectedFile = null;
    this.busForm.reset({ status: 'Available' });
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
      status: bus.status || 'Available'
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
        error: (err) => this.toastService.showError('Lỗi tải ảnh: ' + (err.error?.message || err.message))
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
      status: raw.status,
      isActive: raw.status !== 'Inactive'
    };
    
    if (this.isEditMode && this.currentBusId) {
      this.busService.updateBus(this.currentBusId, { ...busData, id: this.currentBusId }).subscribe({
        next: () => {
          this.toastService.showSuccess('Cập nhật thông tin xe thành công');
          this.loadBuses();
          this.closeModal();
        },
        error: (err) => this.toastService.showError('Lỗi cập nhật: ' + (err.error?.message || err.message))
      });
    } else {
      this.busService.createBus(busData).subscribe({
        next: () => {
          this.toastService.showSuccess('Thêm xe mới thành công');
          this.loadBuses();
          this.closeModal();
        },
        error: (err) => this.toastService.showError('Lỗi tạo mới: ' + (err.error?.message || err.message))
      });
    }
  }

  onDelete(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa xe này?')) {
      this.busService.deleteBus(id).subscribe({
        next: () => {
          this.toastService.showSuccess('Đã xóa xe');
          this.loadBuses();
        },
        error: (err) => this.toastService.showError('Lỗi khi xóa: ' + (err.error?.message || err.message))
      });
    }
  }
}

