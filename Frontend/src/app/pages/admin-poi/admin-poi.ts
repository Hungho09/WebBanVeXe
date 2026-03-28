import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LocationService, LocationModel } from '../../services/location.service';
import { RouteService } from '../../services/route.service';
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
  provinces: string[] = []; // Strings from Route Origins/Destinations
  searchTerm: string = '';

  // Modal state
  showModal = false;
  isEditing = false;
  currentLocation: Partial<LocationModel> = {
    name: '', address: '', provinceName: ''
  };

  loading = false;
  
  // Grouping
  groupedLocations: { provinceName: string, locations: LocationModel[] }[] = [];

  constructor(private locationService: LocationService, private routeService: RouteService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.routeService.getLocations().subscribe(locData => {
      // Merge origins and destinations, get distinct values
      const set = new Set([...locData.origins, ...locData.destinations]);
      this.provinces = Array.from(set).sort();
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
    const groups = new Map<string, { provinceName: string, locations: LocationModel[] }>();
    
    // Khởi tạo groups cho tất cả các tỉnh có trong tuyến đường
    this.provinces.forEach(p => {
      groups.set(p, { provinceName: p, locations: [] });
    });

    // Thêm bucket cho các POI chưa có tỉnh
    groups.set('unassigned', { provinceName: 'Chưa phân bổ', locations: [] });

    this.locations.forEach(loc => {
      const pId = loc.provinceName || 'unassigned';
      if (!groups.has(pId)) {
         groups.set(pId, { provinceName: loc.provinceName || 'Không xác định', locations: []});
      }
      groups.get(pId)?.locations.push(loc);
    });

    // Cập nhật mảng hiển thị (grouping objects)
    this.groupedLocations = Array.from(groups.values()).filter(g => g.locations.length > 0);
  }

  openCreateModal() {
    this.isEditing = false;
    this.currentLocation = { name: '', address: '', provinceName: undefined }; // undefined so that "Chọn tỉnh thành" is shown
    this.showModal = true;
  }

  openEditModal(loc: LocationModel) {
    this.isEditing = true;
    this.currentLocation = { ...loc };
    this.showModal = true;
  }

  saveLocation() {
    if (!this.currentLocation.name || !this.currentLocation.address || !this.currentLocation.provinceName) {
      alert("⚠️ Vui lòng điền đầy đủ các thông tin: Tỉnh/Thành phố, Tên địa điểm, Địa chỉ!");
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
}
