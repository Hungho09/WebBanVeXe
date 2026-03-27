import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookingResponseDto, BookingService } from '../../services/booking.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-admin-cancel-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-cancel-management.html',
  styleUrl: './admin-cancel-management.css'
})
export class AdminCancelManagementComponent implements OnInit {
  private readonly bookingService = inject(BookingService);
  private readonly authService = inject(AuthService);
  private readonly toast = inject(ToastService);

  bookingId = '';
  booking: BookingResponseDto | null = null;
  cancelRequests: BookingResponseDto[] = [];
  loading = false;

  ngOnInit(): void {
    this.loadCancelRequests();
  }

  loadCancelRequests(): void {
    this.loading = true;
    this.bookingService.getCancelRequests().subscribe({
      next: (rows) => {
        this.cancelRequests = rows;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.showError('Khong tai duoc danh sach yeu cau huy.');
      }
    });
  }

  searchBooking(): void {
    if (!this.bookingId.trim()) return;
    this.loading = true;
    this.bookingService.getBooking(this.bookingId.trim()).subscribe({
      next: (b) => {
        this.booking = b;
        this.loading = false;
      },
      error: () => {
        this.booking = null;
        this.loading = false;
        this.toast.showError('Khong tim thay booking.');
      }
    });
  }

  approveCancel(): void {
    if (!this.booking) return;
    const adminUserId = this.authService.getUser().id;
    if (!adminUserId) {
      this.toast.showError('Khong tim thay thong tin admin dang nhap.');
      return;
    }
    this.bookingService.approveCancelBooking(this.booking.id, adminUserId).subscribe({
      next: () => {
        this.toast.showSuccess('Da duyet huy ve thanh cong.');
        this.booking = { ...this.booking!, bookingStatus: 'Cancelled' };
        this.cancelRequests = this.cancelRequests.filter((x) => x.id !== this.booking!.id);
      },
      error: (err) => {
        this.toast.showError(err.error?.message || 'Duyet huy ve that bai.');
      }
    });
  }

  pickBooking(b: BookingResponseDto): void {
    this.booking = b;
    this.bookingId = b.id;
  }

  seatText(b: BookingResponseDto): string {
    return b.details?.length ? b.details.map((d) => d.seatNumber).join(', ') : 'N/A';
  }
}
