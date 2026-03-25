import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CmsService, CmsPopularRoute, CmsPromo, CmsNotice, CmsTestimonial, CmsConfig } from '../../services/cms.service';
import { RouteService } from '../../services/route.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-cms-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cms-management.html',
  styleUrl: './cms-management.css'
})
export class CmsManagement implements OnInit {
  isAdmin = false;
  
  // Advanced Alignment Modal state
  showAlignModal = false;
  alignTarget: any = null;
  alignType: 'route' | 'testimonial' = 'route';
  
  // CMS Data Data
  popularRoutes: CmsPopularRoute[] = [];
  promos: CmsPromo[] = [];
  notices: CmsNotice[] = [];
  testimonials: CmsTestimonial[] = [];

  // Core Data for Linking
  availableRoutes: any[] = [];

  constructor(
    private cmsService: CmsService,
    private routeService: RouteService,
    private authService: AuthService,
    private toast: ToastService
  ) {}

  ngOnInit() {
    this.isAdmin = this.authService.getUser().role === 'Admin';
    if (this.isAdmin) {
      this.loadCmsData();
      this.loadCoreRoutes();
    }
  }

  // --- IMAGE UPLOAD LOGIC ---
  triggerFileUpload(inputId: string) {
    document.getElementById(inputId)?.click();
  }

  onFileSelected(event: any, item: any, type: 'route' | 'testimonial') {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        const fieldName = type === 'route' ? 'imageUrl' : 'avatar';
        item[fieldName] = e.target.result; // Base64 string
        
        // Auto-open alignment modal for the new image
        this.openAlignModal(item, type);
      };
      reader.readAsDataURL(file);
    }
  }

  loadCmsData() {
    this.cmsService.getRemoteConfig().subscribe(config => {
      this.popularRoutes = config.popularRoutes || [];
      this.promos = config.promos || [];
      this.notices = config.notices || [];
      this.testimonials = config.testimonials || [];
    });
  }

  // --- SAVE ALL HELPER ---
  private saveAllChanges(message: string) {
    const fullConfig: CmsConfig = {
      popularRoutes: this.popularRoutes,
      promos: this.promos,
      notices: this.notices,
      testimonials: this.testimonials
    };
    this.cmsService.saveRemoteConfig(fullConfig).subscribe({
      next: () => this.toast.showSuccess(message),
      error: (err: any) => {
        const msg = err.status === 403 ? 'Bạn không có quyền quản trị CMS' : (err.error?.message || 'Không thể lưu dữ liệu lên server');
        this.toast.showError(msg);
      }
    });
  }

  loadCoreRoutes() {
    this.routeService.getRoutes().subscribe({
      next: (routes) => {
        this.availableRoutes = routes;
      },
      error: () => {
        console.error('Không thể tải tuyến đường');
      }
    });
  }

  // --- POPULAR ROUTES ---
  addPopularRoute() {
    this.popularRoutes.push({
      id: Date.now().toString(), routeId: '', routeName: 'Tuyến mới', distanceKm: 0,
      imageUrl: '', mockPrice: '0đ', mockTime: '0 giờ', badgeText: ''
    });
  }
  removePopularRoute(index: number) {
    this.popularRoutes.splice(index, 1);
  }
  savePopularRoutes() {
    this.popularRoutes.forEach(pr => {
      const found = this.availableRoutes.find(r => r.id === pr.routeId);
      if (found) {
        pr.routeName = `${found.origin} -> ${found.destination}`;
        pr.distanceKm = found.distanceKm;
      }
    });
    this.saveAllChanges('Cập nhật Tuyến đường thành công');
  }

  // --- PROMOS ---
  addPromo() {
    this.promos.push({ id: Date.now().toString(), title: 'Ưu đãi mới', description: '', code: '', icon: 'fas fa-star', color: '', buttonText: 'Chi tiết' });
  }
  removePromo(index: number) { this.promos.splice(index, 1); }
  savePromos() {
    this.saveAllChanges('Cập nhật Ưu đãi thành công');
  }

  // --- NOTICES ---
  addNotice() {
    this.notices.push({ id: Date.now().toString(), dateStr: 'Hôm nay', title: 'Thông báo mới', link: '#' });
  }
  removeNotice(index: number) { this.notices.splice(index, 1); }
  saveNotices() {
    this.saveAllChanges('Cập nhật Thông báo thành công');
  }

  // --- TESTIMONIALS ---
  addTestimonial() {
    this.testimonials.push({ id: Date.now().toString(), name: 'Người dùng mới', role: 'Khách hàng', comment: '...', avatar: '' });
  }
  removeTestimonial(index: number) { this.testimonials.splice(index, 1); }
  saveTestimonials() {
    this.saveAllChanges('Đã ghi nhận Đánh giá khách hàng');
  }

  // --- ALIGNMENT MODAL METHODS ---
  openAlignModal(item: any, type: 'route' | 'testimonial') {
    this.alignTarget = item;
    this.alignType = type;
    
    // Set defaults if missing
    if (this.alignTarget.zoom === undefined) this.alignTarget.zoom = 100;
    if (this.alignTarget.offsetX === undefined) this.alignTarget.offsetX = 0;
    if (this.alignTarget.offsetY === undefined) this.alignTarget.offsetY = 0;
    
    this.showAlignModal = true;
  }

  closeAlignModal() {
    this.showAlignModal = false;
    this.alignTarget = null;
  }
}
