import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { BookingResponseDto, BookingService } from '../../services/booking.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-bookings.html',
  styleUrl: './my-bookings.css'
})
export class MyBookingsComponent implements OnInit {
  private readonly bookingService = inject(BookingService);
  private readonly authService = inject(AuthService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  bookings: BookingResponseDto[] = [];
  isLoading = false;
  userId: string | null = null;

  ngOnInit(): void {
    this.userId = this.authService.getUser().id;
    if (!this.userId) {
      this.toast.showWarning('Vui long dang nhap de xem lich su dat ve.');
      this.router.navigate(['/login'], { queryParams: { returnUrl: '/my-bookings' } });
      return;
    }
    this.loadBookings();
  }

  loadBookings(): void {
    if (!this.userId) return;
    this.isLoading = true;
    this.bookingService.getUserBookings(this.userId).subscribe({
      next: (rows) => {
        this.bookings = rows;
        this.isLoading = false;
      },
      error: () => {
        this.toast.showError('Khong the tai danh sach ve.');
        this.isLoading = false;
      }
    });
  }

  canRequestCancel(status: string): boolean {
    return status === 'Paid' || status === 'Pending';
  }

  requestCancel(bookingId: string): void {
    if (!this.userId) return;
    this.bookingService.cancelBooking(bookingId, this.userId).subscribe({
      next: () => {
        this.toast.showSuccess('Da gui yeu cau huy ve.');
        this.bookings = this.bookings.map((b) =>
          b.id === bookingId ? { ...b, bookingStatus: 'CancelRequested' } : b
        );
      },
      error: (err) => {
        this.toast.showError(err.error?.message || 'Gui yeu cau huy ve that bai.');
      }
    });
  }

  statusLabel(status: string): string {
    if (status === 'Pending') return 'Da thanh toan';
    if (status === 'Paid') return 'Da thanh toan';
    if (status === 'CancelRequested') return 'Cho duyet huy';
    if (status === 'Cancelled') return 'Da huy';
    return status;
  }

  statusClass(status: string): string {
    if (status === 'Pending' || status === 'Paid') return 'paid';
    if (status === 'CancelRequested') return 'requested';
    if (status === 'Cancelled') return 'cancelled';
    return 'neutral';
  }

  getSeatSummary(b: BookingResponseDto): string {
    if (!b.details?.length) return 'Chua co thong tin ghe';
    return b.details.map((x) => x.seatNumber).join(', ');
  }

  cancelRequestedCount(): number {
    return this.bookings.filter((b) => b.bookingStatus === 'CancelRequested').length;
  }

  cancelledCount(): number {
    return this.bookings.filter((b) => b.bookingStatus === 'Cancelled').length;
  }
}
