import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

export interface CmsPopularRoute {
  id: string;
  routeId: string;
  routeName: string;
  distanceKm: number;
  imageUrl: string;
  mockPrice: string;
  mockTime: string;
  badgeText: string;
  fitMode?: 'cover' | 'contain' | 'fill';
  zoom?: number;
  offsetX?: number;
  offsetY?: number;
}

export interface CmsPromo {
  id: string;
  title: string;
  description: string;
  code: string;
  icon: string;
  color: string;
  buttonText: string;
}

export interface CmsNotice {
  id: string;
  dateStr: string;
  title: string;
  link: string;
}

export interface CmsTestimonial {
  id: string;
  name: string;
  role: string;
  comment: string;
  avatar: string;
  fitMode?: 'cover' | 'contain' | 'fill';
  zoom?: number;
  offsetX?: number;
  offsetY?: number;
}

export interface CmsConfig {
  popularRoutes: CmsPopularRoute[];
  promos: CmsPromo[];
  notices: CmsNotice[];
  testimonials: CmsTestimonial[];
}

@Injectable({
  providedIn: 'root'
})
export class CmsService {
  private apiUrl = '/api/cms';
  private STORAGE_KEY = 'cms_data_v1';
  private CONFIG_KEY = 'homepage_v1';

  private defaultData: CmsConfig = {
    popularRoutes: [
      { id: '1', routeId: '', routeName: 'Sài Gòn -> Đà Lạt', distanceKm: 305, imageUrl: 'https://img.freepik.com/free-photo/hoi-an-ancient-town-vietnam_268835-1327.jpg?w=740', mockPrice: '250.000đ', mockTime: '8 giờ', badgeText: '-10%' },
      { id: '2', routeId: '', routeName: 'Hà Nội -> Hạ Long', distanceKm: 165, imageUrl: 'https://img.freepik.com/free-photo/beautiful-view-ha-long-bay-vietnam_181624-34539.jpg?w=740', mockPrice: '300.000đ', mockTime: '3 giờ', badgeText: '' },
      { id: '3', routeId: '', routeName: 'Sài Gòn -> Nha Trang', distanceKm: 430, imageUrl: 'https://img.freepik.com/free-photo/nha-trang-beach-city-vietnam_181624-40662.jpg?w=740', mockPrice: '320.000đ', mockTime: '9.5 giờ', badgeText: 'Hot' }
    ],
    promos: [
      { id: '1', title: 'Giảm ngay 50k cho thành viên mới', description: 'Nhập mã VEXE50K khi đặt chuyến đầu tiên qua ứng dụng.', code: 'VEXE50K', icon: 'fas fa-gift', color: '', buttonText: 'Copy Mã' },
      { id: '2', title: 'Hoàn tiền 10% thanh toán VNPAY', description: 'Áp dụng cho mọi chuyến đi khi thanh toán quét mã QR VNPAY.', code: '', icon: 'fas fa-percent', color: 'bg-orange', buttonText: 'Chi tiết' }
    ],
    notices: [
      { id: '1', dateStr: 'Hôm nay', title: 'Mở bán vé xe TẾT 2027 với hàng nghìn ghế Limousine VIP', link: '#' },
      { id: '2', dateStr: 'Hôm qua', title: 'Thông báo thay đổi điểm đón trả khách tại trạm Bến xe Miền Đông mới', link: '#' },
      { id: '3', dateStr: '2 ngày trước', title: 'Cảnh báo tình trạng lừa đảo bán vé xe ảo trên mạng', link: '#' }
    ],
    testimonials: [
      { id: '1', name: 'Nguyễn Hồ Thế Anh', role: 'Chủ tịch HĐQT', comment: '"Lần đầu đồng hành cùng Codenhalam, tôi thực sự ấn tượng bởi tầm nhìn mang lại trải nghiệm tiện lợi tối đa cho mọi chuyến hành trình xuyên suốt Bắc - Nam."', avatar: '/assets/avatar/theanh.jpg', zoom: 100, offsetX: 0, offsetY: 0 },
      { id: '2', name: 'Shark Phi', role: 'Giám đốc BSSC', comment: '"Các đối tác của hệ thống đều là những hãng xe lớn, có uy tín nên tôi hoàn toàn yên tâm khi lựa chọn đặt vé cho bản thân và gia đình. Tốc độ thanh toán siêu nhanh!"', avatar: '', zoom: 100, offsetX: 0, offsetY: 0 }
    ]
  };

  constructor(private http: HttpClient) { }

  // New API Methods
  getRemoteConfig(): Observable<CmsConfig> {
    // Calling with PascalCase to match Controller just in case
    return this.http.get<any>(`/api/Cms/${this.CONFIG_KEY}`).pipe(
      map(res => {
        if (res && res.contentJson && res.contentJson !== '{}') {
          try {
            const config = JSON.parse(res.contentJson);
            this.saveToLocal(config);
            return config;
          } catch (e) {
            console.error('Lỗi parse JSON từ server:', e);
          }
        }
        // If content is empty or invalid, try to use local or default
        return this.getLocalConfig();
      }),
      catchError(err => {
        console.error('Lỗi kết nối API CMS:', err);
        // Still return local as fallback but log properly
        return of(this.getLocalConfig());
      })
    );
  }

  saveRemoteConfig(config: CmsConfig): Observable<any> {
    const dto = {
      configKey: this.CONFIG_KEY,
      contentJson: JSON.stringify(config)
    };
    return this.http.post('/api/Cms', dto).pipe(
      tap(() => this.saveToLocal(config))
    );
  }

  // Local helper methods
  private getLocalConfig(): CmsConfig {
    const data = localStorage.getItem(this.STORAGE_KEY);
    if (data) {
      try { return JSON.parse(data); } catch (e) { return this.defaultData; }
    }
    return this.defaultData;
  }

  private saveToLocal(config: CmsConfig) {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(config));
  }

  // Legacy sync methods (optional - can be updated later in components)
  getConfig(): CmsConfig {
    return this.getLocalConfig();
  }
}
