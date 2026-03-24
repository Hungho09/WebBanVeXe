import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TripService } from '../../services/trip.service';
import { BookingService, CreateBookingDto } from '../../services/booking.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

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
  imports: [CommonModule],
  templateUrl: './booking.html',
  styleUrl: './booking.css'
})
export class Booking implements OnInit, OnDestroy {
  tripId: string | null = null;
  trip: any | null = null;
  seats: Seat[] = [];
  points: any[] = [];
  
  selectedSeatIds: string[] = [];
  selectedPickupId: string | null = null;
  selectedDropoffId: string | null = null;

  activeStep: 'seat' | 'point' | 'info' = 'seat';
  isLoading = true;
  timerValue = 0;
  timerDisplay = '10:00';
  private timerInterval: any;

  // Payment methods
  paymentMethods = [
    { id: 'vnpay', name: 'Thanh toán trực tuyến (VNPay)', icon: 'fas fa-wallet' },
    { id: 'momo', name: 'Ví MoMo', icon: 'fas fa-mobile-alt' },
    { id: 'cash', name: 'Thanh toán khi lên xe', icon: 'fas fa-money-bill-wave' }
  ];
  selectedPaymentMethod = 'vnpay';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tripService: TripService,
    private bookingService: BookingService,
    private authService: AuthService,
    private toastService: ToastService
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
    this.tripService.getSeatsByTrip(tripId).subscribe({
      next: (seats) => {
        this.seats = seats;
        this.isLoading = false;
      },
      error: () => {
        this.toastService.showError('Failed to load seats');
        this.isLoading = false;
      }
    });
  }

  loadPoints(tripId: string): void {
    this.tripService.getTripPoints(tripId).subscribe({
      next: (raw) => {
        this.points = this.normalizeTripPoints(raw);
        queueMicrotask(() => this.applyDefaultPointSelection());
      },
      error: () => {
        this.points = [];
        queueMicrotask(() => this.applyDefaultPointSelection());
      }
    });
  }

  /** API có thể trả camelCase hoặc PascalCase; map về một dạng. */
  private normalizeTripPoints(raw: any[] | null | undefined): any[] {
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
    const pickups = this.pickupPoints;
    if (pickups.length === 1) {
      this.selectedPickupId = this.pointId(pickups[0]);
    }
    const drops = this.dropoffPoints;
    if (drops.length === 1) {
      this.selectedDropoffId = this.pointId(drops[0]);
    } else if (drops.length === 0) {
      this.selectedDropoffId = 'door-to-door';
    }
  }

  get floors(): number[] {
    const floorSet = new Set(this.seats.map(s => s.floor || 1));
    return Array.from(floorSet).sort((a, b) => a - b);
  }

  getSeatsByFloor(floorNum: number): Seat[] {
    return this.seats.filter(s => s.floor === floorNum);
  }

  get pickupPoints(): any[] {
    const api = this.points.filter((p) => p.isPickup);
    return api.length > 0 ? api : this.buildSyntheticPickups();
  }

  /** Chỉ điểm trả từ API (không gồm “Trả tận nơi” cứng trong template). */
  get dropoffPoints(): any[] {
    const api = this.points.filter((p) => p.isDropoff);
    return api.length > 0 ? api : this.buildSyntheticDropoffs();
  }

  toggleSeat(seat: any): void {
    const isAlreadySelected = this.selectedSeatIds.includes(seat.id);

    if (isAlreadySelected) {
      this.bookingService.unlockSeat(seat.id).subscribe({
        next: () => {
          this.selectedSeatIds = this.selectedSeatIds.filter(id => id !== seat.id);
          if (this.selectedSeatIds.length === 0) this.stopTimer();
          this.loadSeats(this.tripId!);
        }
      });
    } else {
      if (seat.status !== 'Available') return;
      this.bookingService.lockSeat(seat.id).subscribe({
        next: () => {
          this.selectedSeatIds.push(seat.id);
          this.startTimer();
          this.loadSeats(this.tripId!);
        },
        error: (err) => this.toastService.showError(err.error?.message || 'Failed to lock seat')
      });
    }
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
    return p?.id != null ? String(p.id) : '';
  }

  showMap(point: any) {
    window.open(`https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(point.name + ' ' + point.address)}`, '_blank');
  }

  get totalAmount(): number {
    return (this.trip?.price || 0) * this.selectedSeatIds.length;
  }

  nextStep() {
    if (this.activeStep === 'seat') {
      if (this.selectedSeatIds.length === 0) {
        this.toastService.showWarning('Vui lòng chọn ít nhất 1 chỗ ngồi!');
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

    const bookingDto: CreateBookingDto = {
      userId: userProfile.id || userProfile.userName || 'unknown', // Handle both id and userName depending on auth implementation
      tripId: this.tripId!,
      seatIds: this.selectedSeatIds
    };

    this.isLoading = true;
    this.bookingService.createBooking(bookingDto).subscribe({
      next: () => {
        this.stopTimer();
        this.toastService.showSuccess('Đặt vé thành công!');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.toastService.showError(err.error?.message || 'Đặt vé thất bại');
        this.isLoading = false;
      }
    });
  }
}
