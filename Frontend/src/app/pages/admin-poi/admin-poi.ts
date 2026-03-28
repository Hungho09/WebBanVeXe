import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LocationService, LocationModel, Province } from '../../services/location.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-poi',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './admin-poi.html',
  styleUrls: ['./admin-poi.css']
})
export class AdminPoiComponent implements OnInit {
  locations: LocationModel[] = [];
  provinces: Province[] = [];
  searchTerm: string = '';

  // Modal state
  showModal = false;
  isEditing = false;
  currentLocation: Partial<LocationModel> = {
    name: '', address: '', isPickup: true, isDropoff: true, isActive: true, isDefault: false
  };

  loading = false;
  
  // Grouping
  groupedLocations: { provinceId?: string, provinceName: string, locations: LocationModel[] }[] = [];

  constructor(private locationService: LocationService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.locationService.getProvinces().subscribe(pt => {
      this.provinces = pt;
      this.fetchLocations();
    });
  }

  fetchLocations() {
    this.locationService.getLocations(this.searchTerm).subscribe(res => {
      this.locations = res;
      this.groupLocations();
      this.loading = false;
    });
  }

  groupLocations() {
    const groups = new Map<string, { provinceId?: string, provinceName: string, locations: LocationModel[] }>();
    
    // Khởi tạo groups cho tất cả các tỉnh (tùy chọn, để hiển thị cả tỉnh chưa có POI)
    this.provinces.forEach(p => {
      groups.set(p.id, { provinceId: p.id, provinceName: p.name, locations: [] });
    });

    // Thêm Tỉnh rỗng cho các POI chưa có tỉnh
    groups.set('unassigned', { provinceName: 'Chưa phân bổ', locations: [] });

    this.locations.forEach(loc => {
      const pId = loc.provinceId || 'unassigned';
      if (!groups.has(pId)) {
         groups.set(pId, { provinceId: loc.provinceId, provinceName: loc.province?.name || 'Không xác định', locations: []});
      }
      groups.get(pId)?.locations.push(loc);
    });

    // Chỉ lấy các group có data để view gọn gàng
    this.groupedLocations = Array.from(groups.values()).filter(g => g.locations.length > 0);
  }

  openCreateModal() {
    this.isEditing = false;
    this.currentLocation = { name: '', address: '', isPickup: true, isDropoff: true, isActive: true, isDefault: false };
    this.showModal = true;
  }

  openEditModal(loc: LocationModel) {
    this.isEditing = true;
    this.currentLocation = { ...loc };
    this.showModal = true;
  }

  saveLocation() {
    if (!this.currentLocation.name || !this.currentLocation.address || !this.currentLocation.provinceId) {
      alert("Vui lòng nhập đủ thông tin (Tên, Địa chỉ, Tỉnh thành)");
      return;
    }

    if (this.isEditing && this.currentLocation.id) {
      this.locationService.updateLocation(this.currentLocation.id, this.currentLocation).subscribe({
        next: () => {
          this.showModal = false;
          this.fetchLocations();
        },
        error: err => alert("Lỗi khi cập nhật")
      });
    } else {
      this.locationService.createLocation(this.currentLocation).subscribe({
        next: () => {
          this.showModal = false;
          this.fetchLocations();
        },
        error: err => alert("Lỗi khi thêm mới")
      });
    }
  }

  deleteLocation(id: string) {
    if(confirm("Bạn có chắc chắn muốn xóa điểm đón/trả này?")) {
       this.locationService.deleteLocation(id).subscribe({
         next: () => this.fetchLocations(),
         error: err => alert("Không thể xóa")
       });
    }
  }

  toggleDefault(id: string) {
    this.locationService.toggleDefault(id).subscribe(() => this.fetchLocations());
  }
}
