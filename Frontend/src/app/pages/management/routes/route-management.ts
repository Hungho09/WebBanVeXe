import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Route, RouteService } from '../../../services/route.service';
import { LocationService, LocationModel } from '../../../services/location.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-route-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './route-management.html',
  styleUrls: ['./route-management.css']
})
export class RouteManagement implements OnInit {
  private readonly toastService = inject(ToastService);
  routeList: Route[] = [];
  filteredList: Route[] = [];
  searchTerm: string = '';
  
  isModalOpen = false;
  routeForm: FormGroup;
  isEditMode = false;
  currentRouteId: string | null = null;
  
  // Point management
  pickupSearch = '';
  dropoffSearch = '';
  pointSort: 'time' | 'distance' = 'time';
  routePoints: any[] = [];
  
  allLocations: LocationModel[] = [];
  selectedLocationId: string = '';
  
  newPoint = {
    type: 'pickup' as 'pickup' | 'dropoff' | 'both',
    offsetMinutes: 0
  };

  provinces = [
    'An Giang', 'Bà Rịa - Vũng Tàu', 'Bắc Giang', 'Bắc Kạn', 'Bạc Liêu', 'Bắc Ninh', 'Bến Tre', 'Bình Định', 
    'Bình Dương', 'Bình Phước', 'Bình Thuận', 'Cà Mau', 'Cần Thơ', 'Cao Bằng', 'Đà Nẵng', 'Đắk Lắk', 'Đắk Nông', 
    'Điện Biên', 'Đồng Nai', 'Đồng Tháp', 'Gia Lai', 'Hà Giang', 'Hà Nam', 'Hà Nội', 'Hà Tĩnh', 'Hải Dương', 
    'Hải Phòng', 'Hậu Giang', 'Hòa Bình', 'Hưng Yên', 'Khánh Hòa', 'Kiên Giang', 'Kon Tum', 'Lai Châu', 
    'Lâm Đồng', 'Lạng Sơn', 'Lào Cai', 'Long An', 'Nam Định', 'Nghệ An', 'Ninh Bình', 'Ninh Thuận', 'Phú Thọ', 
    'Phú Yên', 'Quảng Bình', 'Quảng Nam', 'Quảng Ngãi', 'Quảng Ninh', 'Quảng Trị', 'Sóc Trăng', 'Sơn La', 
    'Tây Ninh', 'Thái Bình', 'Thái Nguyên', 'Thanh Hóa', 'Thừa Thiên Huế', 'Tiền Giang', 'TP Hồ Chí Minh', 
    'Trà Vinh', 'Tuyên Quang', 'Vĩnh Long', 'Vĩnh Phúc', 'Yên Bái'
  ];

  constructor(
    private fb: FormBuilder, 
    private routeService: RouteService,
    private locationService: LocationService,
    private router: Router
  ) {
    this.routeForm = this.fb.group({
      origin: ['', [Validators.required, Validators.minLength(2)]],
      destination: ['', [Validators.required, Validators.minLength(2)]],
      distanceKm: [0], // Bỏ ràng buộc min(1) để dễ dàng tạo tuyến nhanh
      points: [''],
      isActive: [true]
    });
  }

  ngOnInit() {
    this.loadRoutes();
    this.fetchPoisFromGit();
  }

  fetchPoisFromGit() {
    this.locationService.getLocations().subscribe({
      next: (locs) => this.allLocations = locs,
      error: () => this.toastService.showError('Không thể tải danh sách điểm (POI)')
    });
  }

  loadRoutes() {
    this.routeService.getRoutes().subscribe({
      next: (routes) => {
        this.routeList = routes;
        this.onSearch();
      },
      error: () => this.toastService.showError('Không thể tải danh sách tuyến đường')
    });
  }

  onSearch() {
    this.filteredList = this.routeList.filter(item => 
      item.origin.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      item.destination.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  openAddModal() {
    this.isEditMode = false;
    this.currentRouteId = null;
    this.routeForm.reset({ isActive: true, distanceKm: 0 });
    this.routePoints = [];
    this.isModalOpen = true;
  }

  openEditModal(route: Route) {
    this.isEditMode = true;
    this.currentRouteId = route.id;
    this.routeForm.patchValue({
      origin: route.origin,
      destination: route.destination,
      distanceKm: route.distanceKm,
      points: route.points,
      isActive: route.isActive
    });
    
    try {
        this.routePoints = route.points ? JSON.parse(route.points) : [];
    } catch {
        this.routePoints = [];
    }
    
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  saveRoute() {
    if (this.routeForm.valid) {
      const routeData = this.routeForm.value;
      
      if (this.isEditMode && this.currentRouteId) {
        this.routeService.updateRoute(this.currentRouteId, { ...routeData, id: this.currentRouteId }).subscribe({
          next: () => {
            this.toastService.showSuccess('Cập nhật tuyến đường thành công');
            this.loadRoutes();
            this.closeModal();
          },
          error: (err) => {
            const msg = typeof err.error === 'string' ? err.error : (err.error?.message || err.message);
            this.toastService.showError('Lỗi cập nhật: ' + msg);
          }
        });
      } else {
        this.routeService.createRoute(routeData).subscribe({
          next: () => {
            this.toastService.showSuccess('Thêm tuyến đường mới thành công');
            this.loadRoutes();
            this.closeModal();
          },
          error: (err) => {
            const msg = typeof err.error === 'string' ? err.error : (err.error?.message || err.message);
            this.toastService.showError('Lỗi tạo mới: ' + msg);
          }
        });
      }
    } else {
      this.toastService.showWarning('Vui lòng nhập ít nhất Điểm đi và Điểm đến');
    }
  }

  onDelete(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa tuyến đường này?')) {
      this.routeService.deleteRoute(id).subscribe({
        next: () => {
          this.toastService.showSuccess('Đã xóa tuyến đường');
          this.loadRoutes();
        },
        error: (err) => this.toastService.showError('Lỗi khi xóa: ' + (err.error?.message || err.message))
      });
    }
  }

  addPoint() {
    const poi = this.allLocations.find(l => l.id === this.selectedLocationId);
    if (!poi) return;
    
    const point = {
        id: crypto.randomUUID(),
        locationId: poi.id, 
        name: poi.name,
        address: poi.address,
        offsetMinutes: this.newPoint.offsetMinutes,
        isPickup: this.newPoint.type === 'pickup' || this.newPoint.type === 'both',
        isDropoff: this.newPoint.type === 'dropoff' || this.newPoint.type === 'both'
    };
    
    this.routePoints.push(point);
    this.updatePointsField();
    
    this.selectedLocationId = '';
    this.newPoint.offsetMinutes = 0;
    this.toastService.showInfo(`Đã thêm điểm: ${poi.name}`);
  }

  removePoint(id: string) {
    this.routePoints = this.routePoints.filter(p => p.id !== id);
    this.updatePointsField();
  }

  updatePointsField() {
    this.routeForm.patchValue({ points: JSON.stringify(this.routePoints) });
  }

  get filteredPickups() {
    let list = this.routePoints.filter(p => p.isPickup);
    if (this.pickupSearch) {
        list = list.filter(p => p.name.toLowerCase().includes(this.pickupSearch.toLowerCase()));
    }
    return list.sort((a,b) => a.offsetMinutes - b.offsetMinutes);
  }

  get filteredDropoffs() {
    let list = this.routePoints.filter(p => p.isDropoff);
    if (this.dropoffSearch) {
        list = list.filter(p => p.name.toLowerCase().includes(this.dropoffSearch.toLowerCase()));
    }
    return list.sort((a,b) => a.offsetMinutes - b.offsetMinutes);
  }

  copyId(id: string) {
    navigator.clipboard.writeText(id).then(() => {
      this.toastService.showInfo(`Đã sao chép ID vào bộ nhớ tạm`);
    });
  }

  navigateTo(url: string) {
    this.router.navigateByUrl(url);
  }
}
