import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Route, RouteService } from '../../services/route.service';

@Component({
  selector: 'app-route-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './route-management.html',
  styleUrls: ['./route-management.css']
})
export class RouteManagement implements OnInit {
  routeList: Route[] = [];
  filteredList: Route[] = [];
  searchTerm: string = '';
  
  isModalOpen = false;
  routeForm: FormGroup;
  isEditMode = false;
  currentRouteId: string | null = null;

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
    private router: Router
  ) {
    this.routeForm = this.fb.group({
      origin: ['', [Validators.required, Validators.minLength(2)]],
      destination: ['', [Validators.required, Validators.minLength(2)]],
      distanceKm: [0, [Validators.required, Validators.min(1)]],
      points: [''],
      isActive: [true]
    });
  }

  ngOnInit() {
    this.loadRoutes();
  }

  loadRoutes() {
    this.routeService.getRoutes().subscribe({
      next: (routes) => {
        this.routeList = routes;
        this.onSearch();
      },
      error: (err) => console.error('Error loading routes', err)
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
            this.loadRoutes();
            this.closeModal();
          },
          error: (err) => {
            const msg = typeof err.error === 'string' ? err.error : (err.error?.message || err.message);
            alert('Lỗi khi cập nhật: ' + msg);
          }
        });
      } else {
        this.routeService.createRoute(routeData).subscribe({
          next: () => {
            this.loadRoutes();
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
    if (confirm('Bạn có chắc chắn muốn xóa tuyến đường này?')) {
      this.routeService.deleteRoute(id).subscribe({
        next: () => this.loadRoutes(),
        error: (err) => alert('Lỗi khi xóa: ' + (err.error?.message || err.message))
      });
    }
  }

  copyId(id: string) {
    navigator.clipboard.writeText(id).then(() => {
      // Flash feedback
      const btn = document.querySelector(`[title="Sao chép ID đầy đủ"][data-id="${id}"]`);
      alert(`Đã sao chép ID: ${id}`);
    });
  }

  navigateTo(url: string) {
    this.router.navigateByUrl(url);
  }
}
