import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BookingService, BookingResponseDto } from '../../services/booking.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  currentDate = new Date();
  bookings: BookingResponseDto[] = [];
  pendingCancellations: BookingResponseDto[] = [];
  user: any;
  isLoading = false;

  constructor(
    private router: Router,
    private bookingService: BookingService,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.user = this.authService.getUser();
    this.loadData();
  }

  loadData() {
    this.isLoading = true;
    if (this.user.role === 'Admin') {
      this.loadAdminData();
    } else if (this.user.id) {
      this.loadCustomerData();
    }
  }

  loadCustomerData() {
    this.bookingService.getUserBookings(this.user.id).subscribe({
      next: (data) => {
        this.bookings = data;
        this.isLoading = false;
      },
      error: () => {
        this.toastService.showError('Failed to load your bookings');
        this.isLoading = false;
      }
    });
  }

  loadAdminData() {
    this.bookingService.getPendingCancellations().subscribe({
      next: (data) => {
        this.pendingCancellations = data;
        this.isLoading = false;
      },
      error: () => {
        this.toastService.showError('Failed to load pending cancellations');
        this.isLoading = false;
      }
    });
  }

  requestCancel(id: string) {
    if (confirm('Are you sure you want to request a cancellation for this booking?')) {
      this.bookingService.requestCancel(id).subscribe({
        next: () => {
          this.toastService.showSuccess('Cancellation request sent');
          this.loadData();
        },
        error: (err) => {
          this.toastService.showError(err.error?.message || 'Failed to request cancellation');
        }
      });
    }
  }

  approveCancel(id: string) {
    if (confirm('Are you sure you want to approve this cancellation request?')) {
      this.bookingService.approveCancel(id).subscribe({
        next: () => {
          this.toastService.showSuccess('Cancellation approved and seats released');
          this.loadData();
        },
        error: (err) => {
          this.toastService.showError('Failed to approve cancellation');
        }
      });
    }
  }

  navigateTo(url: string) {
    this.router.navigateByUrl(url);
  }
}
