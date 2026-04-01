import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TripService } from '../../../services/trip.service';
import { BookingService, CreateBookingDto } from '../../../services/booking.service';
import { AuthService } from '../../../services/auth.service';
import { ToastService } from '../../../services/toast.service';
import { InvoiceService } from '../../../services/invoice.service';
import { InvoiceExportDialogComponent } from '../../../components/invoice-export-dialog/invoice-export-dialog.component';

export interface Point {
  id: string;
  name: string;
  address: string;
  expectedTime: string;
  distanceFromOrigin: number;
  badge?: string;
  isPickup: boolean;
  isDropoff: boolean;
  latitude?: number;
  longitude?: number;
  distanceToUser?: number;
}

export interface Seat {
  id: string;
  seatNumber: string;
  status: string;
  floor?: number;
  rowNumber?: number;
  columnNumber?: number;
  type?: string;
}

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, FormsModule, InvoiceExportDialogComponent],
  templateUrl: './booking.html',
  styleUrl: './booking.css'
})
export class Booking implements OnInit, OnDestroy {
  tripId: string | null = null;
  trip: any | null = null;
  seats: Seat[] = [];
  points: Point[] = [];
  
  selectedSeatIds: string[] = [];
  selectedPickupId: string | null = null;
  selectedDropoffId: string | null = null;

  activeStep: 'seat' | 'point' | 'info' = 'seat';
  isLoading = true;
  timerValue = 0;
  timerDisplay = '10:00';
  private timerInterval: any;
  private readonly guidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

  pickupSearch = '';
  dropoffSearch = '';
  pickupSort: 'time' | 'distance' = 'time';
  dropoffSort: 'time' | 'distance' = 'time';
  userCoords: { lat: number; lng: number } | null = null;

  // Payment methods
  paymentMethods = [
    { 
      id: 'vnpay', 
      name: 'Thanh toán trực tuyến (VNPay)', 
      icon: 'fas fa-wallet',
      description: 'An toàn, nhanh chóng, hỗ trợ nhiều ngân hàng'
    },
    { 
      id: 'momo', 
      name: 'Ví MoMo', 
      icon: 'fas fa-mobile-alt',
      description: 'Thanh toán bằng ví điện tử MoMo'
    },
    { 
      id: 'cash', 
      name: 'Thanh toán khi lên xe', 
      icon: 'fas fa-money-bill-wave',
      description: 'Trả tiền mặt trực tiếp cho tài xế'
    }
  ];
  selectedPaymentMethod = 'vnpay';

  // Invoice export dialog
  showInvoiceDialog = false;
  invoiceInfo: any = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tripService: TripService,
    private bookingService: BookingService,
    public authService: AuthService,
    private toastService: ToastService,
    private invoiceService: InvoiceService
  ) {}

  ngOnInit(): void {
    this.tripId = this.route.snapshot.paramMap.get('id');
    
    
    if (this.tripId) {
      this.loadTripData(this.tripId);
    } else {
      this.toastService.showError('Invalid Trip ID');
      this.router.navigate(['/homepage']);
    }
  }

  ngOnDestroy(): void {
    this.stopTimer();
  }

  loadTripData(id: string): void {
    this.isLoading = true;
    this.tripService.getTrip(id).subscribe({
      next: (trip) => {
        this.trip = trip;
        this.loadSeats(id);
        this.loadPoints(id);
      },
      error: () => {
        this.toastService.showError('Failed to load trip details');
        this.isLoading = false;
      }
    });
  }

  loadSeats(tripId: string): void {
    const userId = this.authService.getUser()?.id;
    this.tripService.getSeatsByTrip(tripId).subscribe({
      next: (seats: any[]) => {
        this.seats = seats;
        
        // Auto-select seats locked by me (across refreshes)
        if (userId) {
            seats.forEach(s => {
                if (s.status === 'Locked' && s.lockedByUserId === userId && !this.selectedSeatIds.includes(s.id)) {
                    this.selectedSeatIds.push(s.id);
                    this.startTimer();
                }
            });
        }
        
        this.isLoading = false;
      },
      error: () => {
        this.toastService.showError('Failed to load seats');
        this.isLoading = false;
      }
    });
  }

  loadPoints(tripId: string): void {
    this.isLoading = true;
    this.tripService.getTripPoints(tripId).subscribe({
      next: (raw) => {
        this.points = this.normalizeTripPoints(raw);
        this.getUserLocation();
        this.applyDefaultPointSelection();
        this.isLoading = false;
      },
      error: () => {
        this.points = [];
        this.isLoading = false;
      }
    });
  }

  getUserLocation() {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition((pos) => {
        this.userCoords = { lat: pos.coords.latitude, lng: pos.coords.longitude };
        this.calculateDistances();
      });
    }
  }

  calculateDistances() {
    if (!this.userCoords) return;
    this.points.forEach(p => {
      if (p.latitude && p.longitude) {
        p.distanceToUser = this.getHaversineDistance(
          this.userCoords!.lat, this.userCoords!.lng,
          p.latitude, p.longitude
        );
      }
    });
  }

  getHaversineDistance(lat1: number, lon1: number, lat2: number, lon2: number): number {
    const R = 6371; // km
    const dLat = (lat2 - lat1) * Math.PI / 180;
    const dLon = (lon2 - lon1) * Math.PI / 180;
    const a = Math.sin(dLat/2) * Math.sin(dLat/2) +
              Math.cos(lat1*Math.PI/180) * Math.cos(lat2*Math.PI/180) * 
              Math.sin(dLon/2) * Math.sin(dLon/2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
    return R * c;
  }

  private normalizeTripPoints(raw: any[] | null | undefined): Point[] {
    if (!raw?.length) return [];
    return raw.map((p) => ({
      id: p.id ?? p.Id,
      name: p.name ?? p.Name ?? '',
      address: p.address ?? p.Address ?? '',
      expectedTime: p.expectedTime ?? p.ExpectedTime,
      distanceFromOrigin: p.distanceFromOrigin ?? p.DistanceFromOrigin ?? 0,
      badge: p.badge ?? p.Badge,
      isPickup: !!(p.isPickup ?? p.IsPickup),
      isDropoff: !!(p.isDropoff ?? p.IsDropoff),
      latitude: p.latitude ?? p.Latitude,
      longitude: p.longitude ?? p.Longitude
    }));
  }

  /** Khi backend chưa seed RouteStops: tạo điểm đón/trả từ routeName + giờ chuyến. */
  private buildSyntheticPickups(): any[] {
    const parts = this.parseRouteEnds();
    const origin = parts[0] || 'Điểm đi';
    const dep = this.trip?.departureTime;
    return [
      {
        id: 'synthetic-pickup-main',
        name: `Đón tại ${origin}`,
        address: `Điểm xuất phát — ${origin} (theo giờ xe)`,
        expectedTime: dep,
        badge: 'Mặc định',
        isPickup: true,
        isDropoff: false,
      },
    ];
  }

  private buildSyntheticDropoffs(): any[] {
    const parts = this.parseRouteEnds();
    const dest = parts[1] || parts[0] || 'Điểm đến';
    const arr = this.trip?.arrivalTime;
    return [
      {
        id: 'synthetic-dropoff-terminal',
        name: `Trả tại ${dest}`,
        address: `Bến / điểm đến — ${dest}`,
        expectedTime: arr,
        badge: null,
        isPickup: false,
        isDropoff: true,
      },
    ];
  }

  private parseRouteEnds(): [string, string] {
    const name = (this.trip?.routeName || '').trim();
    if (!name) return ['', ''];
    const bits = name.split(/\s*-\s*/).map((s: string) => s.trim()).filter(Boolean);
    if (bits.length >= 2) return [bits[0], bits[bits.length - 1]];
    return [bits[0] || '', ''];
  }

  private applyDefaultPointSelection(): void {
    if (this.filteredPickups.length > 0) {
      this.selectedPickupId = this.filteredPickups[0].id;
    }
    if (this.filteredDropoffs.length > 0) {
      this.selectedDropoffId = this.filteredDropoffs[0].id;
    } else {
      this.selectedDropoffId = 'door-to-door';
    }
  }

  // ── Vehicle type detection ──
  get vehicleCategory(): 'seat' | 'limousine' | 'sleeper' | 'cabin-single' | 'cabin-double' {
    const name = (this.trip?.busTypeName || '').toLowerCase();
    if (name.includes('limousine')) return 'limousine';
    if ((name.includes('giường phòng') || name.includes('cabin')) && name.includes('đôi')) return 'cabin-double';
    if (name.includes('giường phòng') || name.includes('cabin single')) return 'cabin-single';
    if (name.includes('giường nằm') || name.includes('sleeper')) return 'sleeper';
    return 'seat';
  }

  get seatLabel(): string {
    switch (this.vehicleCategory) {
      case 'sleeper': return 'Chọn giường nằm';
      case 'cabin-single': return 'Chọn phòng đơn';
      case 'cabin-double': return 'Chọn phòng đôi';
      case 'limousine': return 'Chọn ghế VIP';
      default: return 'Chọn chỗ ngồi';
    }
  }

  get seatIcon(): string {
    switch (this.vehicleCategory) {
      case 'sleeper': return 'fas fa-bed';
      case 'cabin-single': return 'fas fa-door-open';
      case 'cabin-double': return 'fas fa-door-open';
      case 'limousine': return 'fas fa-crown';
      default: return 'fas fa-chair';
    }
  }

  get seatUnit(): string {
    switch (this.vehicleCategory) {
      case 'sleeper': return 'giường';
      case 'cabin-single': return 'phòng';
      case 'cabin-double': return 'phòng';
      default: return 'ghế';
    }
  }

  get seatHint(): string {
    switch (this.vehicleCategory) {
      case 'sleeper': return 'Nhấn vào giường trống để chọn. Tối đa 5 giường/lượt.';
      case 'cabin-single': return 'Nhấn vào phòng trống để chọn. Tối đa 5 phòng/lượt.';
      case 'cabin-double': return 'Nhấn vào phòng trống để chọn. Tối đa 5 phòng/lượt.';
      case 'limousine': return 'Nhấn vào ghế VIP trống để chọn. Tối đa 5 ghế/lượt.';
      default: return 'Nhấn vào ghế trống để chọn. Tối đa 5 ghế/lượt.';
    }
  }

  floorLabel(floor: number): string {
    if (this.floors.length <= 1) return '';
    switch (this.vehicleCategory) {
      case 'sleeper': return floor === 1 ? 'Tầng dưới' : 'Tầng trên';
      case 'cabin-single':
      case 'cabin-double': return floor === 1 ? 'Tầng 1' : 'Tầng 2';
      default: return `Tầng ${floor}`;
    }
  }

  get floors(): number[] {
    const floorSet = new Set(this.seats.map(s => s.floor || 1));
    return Array.from(floorSet).sort((a, b) => a - b);
  }

  get maxColumns(): number {
    if (!this.seats || this.seats.length === 0) return 3;
    const max = Math.max(...this.seats.map(s => s.columnNumber || 0));
    return max > 0 ? max : 3;
  }

  getSeatsByFloor(floorNum: number): Seat[] {
    return this.seats.filter(s => s.floor === floorNum);
  }

  get filteredPickups(): Point[] {
    let list = [...this.points.filter(p => p.isPickup)];
    if (this.pickupSearch) {
      const s = this.pickupSearch.toLowerCase();
      list = list.filter(p => p.name.toLowerCase().includes(s) || p.address.toLowerCase().includes(s));
    }
    
    return list.sort((a, b) => {
      if (this.pickupSort === 'distance' && a.distanceToUser !== undefined && b.distanceToUser !== undefined) {
        return a.distanceToUser - b.distanceToUser;
      }
      return new Date(a.expectedTime).getTime() - new Date(b.expectedTime).getTime();
    });
  }
  
  get filteredDropoffs(): Point[] {
    let list = [...this.points.filter(p => p.isDropoff)];
    
    // Add special options
    const doorToDoor: Point = {
        id: 'door-to-door',
        name: 'Trả tận nơi',
        address: 'Hỗ trợ trả khách tận nơi trong nội thành',
        expectedTime: this.trip?.arrivalTime,
        distanceFromOrigin: 0,
        badge: 'Phổ biến',
        isPickup: false,
        isDropoff: true
    };
    
    // Logic for shuttle
    const shuttle: Point = {
        id: 'shuttle-dropoff',
        name: 'Trung chuyển',
        address: 'Trung chuyển miễn phí trong bán kính 5km',
        expectedTime: this.trip?.arrivalTime,
        distanceFromOrigin: 0,
        badge: 'Miễn phí',
        isPickup: false,
        isDropoff: true
    };

    if (!this.dropoffSearch) {
        list.unshift(shuttle);
        list.unshift(doorToDoor);
    } else {
        const s = this.dropoffSearch.toLowerCase();
        if (doorToDoor.name.toLowerCase().includes(s)) list.unshift(doorToDoor);
        if (shuttle.name.toLowerCase().includes(s)) list.unshift(shuttle);
    }

    if (this.dropoffSearch) {
      const s = this.dropoffSearch.toLowerCase();
      list = list.filter(p => p.name.toLowerCase().includes(s) || p.address.toLowerCase().includes(s));
    }
    
    return list.sort((a, b) => {
      // Keep special at top
      const specials = ['door-to-door', 'shuttle-dropoff'];
      if (specials.includes(a.id) && !specials.includes(b.id)) return -1;
      if (!specials.includes(a.id) && specials.includes(b.id)) return 1;
      
      if (this.dropoffSort === 'distance' && a.distanceToUser !== undefined && b.distanceToUser !== undefined) {
        return a.distanceToUser - b.distanceToUser;
      }
      return new Date(a.expectedTime).getTime() - new Date(b.expectedTime).getTime();
    });
  }

  toggleSeat(seat: any): void {
    const isAlreadySelected = this.selectedSeatIds.includes(seat.id);

    if (isAlreadySelected) {
      const uid = this.getCurrentUserId();
      if (!uid) {
        this.toastService.showWarning('Vui lòng đăng nhập để thao tác ghế.');
        return;
      }
      this.bookingService.unlockSeat(seat.id, uid).subscribe({
        next: () => {
          this.selectedSeatIds = this.selectedSeatIds.filter(id => id !== seat.id);
          if (this.selectedSeatIds.length === 0) this.stopTimer();
          this.loadSeats(this.tripId!);
        }
      });
    } else {
      if (seat.status !== 'Available') return;
      const uid = this.getCurrentUserId();
      if (!uid) {
        this.toastService.showWarning('Vui lòng đăng nhập để chọn ghế.');
        return;
      }
      this.bookingService.lockSeat(seat.id, uid).subscribe({
        next: () => {
          this.selectedSeatIds.push(seat.id);
          this.startTimer();
          this.loadSeats(this.tripId!);
        },
        error: (err) => this.toastService.showError(err.error?.message || 'Failed to lock seat')
      });
    }
  }

  private getCurrentUserId(): string | null {
    const id = this.authService.getUser().id;
    if (!id || !this.guidPattern.test(id)) return null;
    return id;
  }

  startTimer(): void {
    if (this.timerInterval) return;
    this.timerValue = 10 * 60;
    this.timerInterval = setInterval(() => {
      this.timerValue--;
      const minutes = Math.floor(this.timerValue / 60);
      const seconds = this.timerValue % 60;
      this.timerDisplay = `${minutes}:${seconds.toString().padStart(2, '0')}`;
      if (this.timerValue <= 0) {
        this.stopTimer();
        this.selectedSeatIds = [];
        this.loadSeats(this.tripId!);
      }
    }, 1000);
  }

  stopTimer(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }
  }

  isSelected(seatId: string): boolean {
    return this.selectedSeatIds.includes(seatId);
  }

  pointId(p: any): string {
    return p?.id != null ? String(p.id) : p;
  }

  showMap(point: any) {
    window.open(`https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(point.name + ' ' + point.address)}`, '_blank');
  }

  get totalAmount(): number {
    return (this.trip?.price || 0) * this.selectedSeatIds.length;
  }

  get selectedSeatNumbers(): string {
    return this.seats
      .filter(s => this.selectedSeatIds.includes(s.id))
      .map(s => s.seatNumber)
      .join(', ');
  }

  get selectedPickupName(): string | undefined {
    return this.points.find(p => p.id === this.selectedPickupId)?.name;
  }

  get selectedDropoffName(): string | undefined {
    if (this.selectedDropoffId === 'door-to-door') return 'Trả tận nơi';
    return this.points.find(p => p.id === this.selectedDropoffId)?.name;
  }

  formatTripTime(iso: string | undefined, onlyTime: boolean = false): string {
    if (!iso) return '--:--';
    const d = new Date(iso);
    if (onlyTime) {
      return d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false });
    }
    return d.toLocaleString('vi-VN', { 
      day: '2-digit', month: '2-digit', 
      hour: '2-digit', minute: '2-digit', 
      hour12: false 
    });
  }

  nextStep() {
    if (this.activeStep === 'seat') {
      if (this.selectedSeatIds.length === 0) {
        this.toastService.showWarning('Vui lòng chọn ít nhất 1 chỗ ngồi!');
        return;
      }
      if (this.selectedSeatIds.length > 5) {
        this.toastService.showWarning('Bạn chỉ được chọn tối đa 5 ghế mỗi lượt đặt!');
        return;
      }
      this.activeStep = 'point';
    } else if (this.activeStep === 'point') {
      if (!this.selectedPickupId || !this.selectedDropoffId) {
        this.toastService.showWarning('Vui lòng chọn Điểm đón và Điểm trả!');
        return;
      }
      this.activeStep = 'info';
    }
  }

  prevStep() {
    if (this.activeStep === 'point') this.activeStep = 'seat';
    else if (this.activeStep === 'info') this.activeStep = 'point';
  }

  confirmBooking(): void {
    const userProfile = this.authService.getUser();
    if (!this.authService.isLoggedIn() || !userProfile) {
      this.toastService.showInfo('Vui lòng đăng nhập để đặt vé');
      this.router.navigate(['/login'], { queryParams: { returnUrl: `/booking/${this.tripId}` } });
      return;
    }

    const uid = userProfile.id ?? '';
    const isGuid = (v: unknown) =>
      typeof v === 'string' &&
      /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$/.test(v);

    if (!isGuid(uid)) {
      this.toastService.showError('Không lấy được UserId hợp lệ. Vui lòng đăng nhập lại để đặt vé.');
      this.router.navigate(['/login'], { queryParams: { returnUrl: `/booking/${this.tripId}` } });
      return;
    }

    const bookingDto: CreateBookingDto = {
      userId: uid,
      tripId: this.tripId!,
      seatIds: this.selectedSeatIds,
      pickupPointId: isGuid(this.selectedPickupId) ? this.selectedPickupId! : undefined,
      dropoffPointId: isGuid(this.selectedDropoffId) ? this.selectedDropoffId! : undefined
    };

    this.isLoading = true;
    this.bookingService.createBooking(bookingDto).subscribe({
      next: (res) => {
        // Step 2: Auto-simulate payment for demo purposes to transition status to Paid
        this.bookingService.processPayment(res.id, this.selectedPaymentMethod).subscribe({
          next: (payRes) => {
            this.stopTimer();
            this.toastService.showSuccess('Thanh toán thành công!');
            
            // Prepare invoice info for dialog
            this.invoiceInfo = {
              invoiceNumber: payRes.transactionCode || `INV${Date.now().toString().slice(-6)}`,
              totalAmount: this.totalAmount,
              customerName: userProfile.userName || 'Khách hàng',
              customerEmail: userProfile.email || '',
              bookingId: res.id
            };
            
            // Show invoice export dialog
            this.showInvoiceDialog = true;
            this.isLoading = false;
          },
          error: (payErr) => {
            console.error('Payment auto-simulate failed:', payErr);
            this.toastService.showWarning('Đặt vé thành công nhưng chưa thể hoàn tất thanh toán tự động.');
            this.isLoading = false;
            this.router.navigate(['/my-bookings']);
          }
        });
      },
      error: (err) => {
        const msg = err?.error?.message || err?.message || 'Đặt vé thất bại';
        this.toastService.showError(msg);
        this.isLoading = false;
        // If concurrency error, reload seats
        if (msg.includes('người khác đặt') || msg.includes('chỗ đã hết')) {
          this.loadSeats(this.tripId!);
          this.activeStep = 'seat';
        }
      }
    });
  }

  // Invoice dialog handlers
  onExportInvoice(): void {
    if (!this.invoiceInfo) return;
    
    
    
    // Create invoice first, then navigate to PDF page
    this.invoiceService.createInvoiceByBookingId(this.invoiceInfo.bookingId || 'temp').subscribe({
      next: (invoice) => {
        
        this.toastService.showSuccess('Tạo hóa đơn thành công!');
        this.showInvoiceDialog = false;
        // Navigate to PDF page instead of downloading
        this.router.navigate(['/invoice-pdf', invoice.id]);
      },
      error: (err) => {
        console.error('Error creating invoice:', err);
        const errorMessage = err?.error?.message || err?.message || 'Lỗi khi tạo hóa đơn. Vui lòng thử lại.';
        this.toastService.showError(errorMessage);
      }
    });
  }

  onGoHome(): void {
    this.showInvoiceDialog = false;
    this.router.navigate(['/homepage']);
  }
}
