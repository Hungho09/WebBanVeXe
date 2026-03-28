import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LocationService, LocationModel } from '../../services/location.service';
import { RouteService } from '../../services/route.service';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';

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

  createData = {
    provinceName: undefined as string | undefined,
    points: [{ name: '', addresses: [''], mapLink: '' }]
  };

  // Autocomplete
  showSuggestions = false;
  filteredSuggestions: LocationModel[] = [];

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

  trackByIndex(index: number, item: any): number {
    return index;
  }

  // Autocomplete interactions
  onSearchTyping() {
    if (!this.searchTerm.trim()) {
      this.filteredSuggestions = [];
      return;
    }
    const term = this.searchTerm.toLowerCase();
    this.filteredSuggestions = this.locations.filter(l => 
      l.name.toLowerCase().includes(term) || 
      l.address.toLowerCase().includes(term) ||
      (l.provinceName && l.provinceName.toLowerCase().includes(term))
    ).slice(0, 10);
  }

  selectSuggestion(s: LocationModel) {
    this.searchTerm = s.name;
    this.showSuggestions = false;
    this.fetchLocations();
  }

  hideSuggestions() {
    setTimeout(() => this.showSuggestions = false, 200);
  }

  // Points mapping
  addPoint() {
    this.createData.points.push({ name: '', addresses: [''], mapLink: '' });
  }

  removePoint(index: number) {
    this.createData.points.splice(index, 1);
  }

  addAddress(pointIndex: number) {
    this.createData.points[pointIndex].addresses.push('');
  }

  removeAddress(pointIndex: number, addrIndex: number) {
    this.createData.points[pointIndex].addresses.splice(addrIndex, 1);
  }

  openCreateModal() {
    this.isEditing = false;
    this.createData = {
      provinceName: undefined,
      points: [{ name: '', addresses: [''], mapLink: '' }]
    };
    this.showModal = true;
  }

  openEditModal(loc: LocationModel) {
    this.isEditing = true;
    this.currentLocation = { ...loc };
    this.showModal = true;
  }

  saveLocation() {
    if (this.isEditing) {
      if (!this.currentLocation.name || !this.currentLocation.address || !this.currentLocation.provinceName) {
        alert("⚠️ Vui lòng điền đầy đủ các thông tin: Tỉnh/Thành phố, Tên địa điểm, Địa chỉ!");
        return;
      }

      this.locationService.updateLocation(this.currentLocation.id!, this.currentLocation).subscribe({
        next: () => {
          this.showModal = false;
          this.fetchLocations();
        },
        error: err => alert("Lỗi khi cập nhật")
      });
    } else {
      if (!this.createData.provinceName || this.createData.points.length === 0) {
        alert("⚠️ Vui lòng chọn Tỉnh/Thành phố và thêm ít nhất 1 điểm.");
        return;
      }
      
      const payload: Partial<LocationModel>[] = [];
      for (const p of this.createData.points) {
         const validAddresses = p.addresses.filter(a => a.trim() !== '');
         if (!p.name.trim() || validAddresses.length === 0) {
            alert("⚠️ Mỗi điểm phải có Tên và ít nhất 1 Địa chỉ hợp lệ.");
            return;
         }
         payload.push({
            name: p.name,
            address: validAddresses.join(' | '),
            provinceName: this.createData.provinceName,
            mapLink: p.mapLink
         });
      }

      const requests = payload.map(data => this.locationService.createLocation(data as any));
      forkJoin(requests).subscribe({
         next: () => {
            this.showModal = false;
            this.fetchLocations();
         },
         error: () => alert("Lỗi trong quá trình thêm điểm mới")
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
