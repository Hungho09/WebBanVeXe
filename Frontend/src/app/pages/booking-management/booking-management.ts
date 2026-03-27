import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BookingService, BookingResponseDto } from '../../services/booking.service';
import { InvoiceService, CreateInvoiceRequest } from '../../services/invoice.service';
import { ToastService } from '../../services/toast.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-booking-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking-management.html',
  styleUrl: './booking-management.css'
})
export class BookingManagement implements OnInit {
  bookings: BookingResponseDto[] = [];
  filteredBookings: BookingResponseDto[] = [];
  searchTerm: string = '';
  isLoading = false;

  constructor(
    private bookingService: BookingService,
    private invoiceService: InvoiceService,
    private toast: ToastService,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.loadBookings();
  }

  loadBookings() {
    this.isLoading = true;
    this.bookingService.getAllBookings().subscribe({
      next: (data) => {
        this.bookings = data;
        this.applyFilter();
        this.isLoading = false;
      },
      error: () => {
        this.toast.showError('Không thể tải danh sách đặt vé');
        this.isLoading = false;
      }
    });
  }

  applyFilter() {
    if (!this.searchTerm) {
      this.filteredBookings = [...this.bookings];
    } else {
      const s = this.searchTerm.toLowerCase();
      this.filteredBookings = this.bookings.filter(b => 
        b.userName.toLowerCase().includes(s) || 
        b.id.toLowerCase().includes(s)
      );
    }
  }

  getStatusClass(status: string) {
    switch (status) {
      case 'Pending': return 'status-pending';
      case 'Paid': return 'status-paid';
      case 'Cancelled': return 'status-cancelled';
      case 'CancelRequested': return 'status-requested';
      default: return '';
    }
  }

  getStatusText(status: string) {
    switch (status) {
      case 'Pending': return 'Chờ thanh toán';
      case 'Paid': return 'Đã thanh toán';
      case 'Cancelled': return 'Đã hủy';
      case 'CancelRequested': return 'Yêu cầu hủy';
      default: return status;
    }
  }

  approveCancel(id: string) {
    if (confirm('Duyệt yêu cầu hủy vé này? Ghế sẽ được giải phóng.')) {
      const adminUserId = this.authService.getUser().id;
      if (!adminUserId) {
        this.toast.showError('Không tìm thấy thông tin admin đăng nhập');
        return;
      }
      this.bookingService.approveCancelBooking(id, adminUserId).subscribe({
        next: () => {
          this.toast.showSuccess('Đã duyệt hủy vé thành công');
          this.loadBookings();
        },
        error: (err) => this.toast.showError(err.error?.message || 'Lỗi khi duyệt hủy')
      });
    }
  }

  cancelBooking(id: string) {
    if (confirm('Bạn có chắc chắn muốn hủy vé này?')) {
      const userId = this.authService.getUser().id;
      if (!userId) {
        this.toast.showError('Không tìm thấy thông tin người dùng');
        return;
      }
      this.bookingService.cancelBooking(id, userId).subscribe({
        next: () => {
          this.toast.showSuccess('Đã hủy vé');
          this.loadBookings();
        },
        error: (err) => this.toast.showError(err.error?.message || 'Lỗi khi hủy')
      });
    }
  }

  viewInvoice(invoiceId: string) {
    this.router.navigate(['/invoices', invoiceId]);
  }

  generateInvoice(bookingId: string) {
    const request: CreateInvoiceRequest = { bookingId };
    
    this.invoiceService.createInvoice(request).subscribe({
      next: (invoice) => {
        this.toast.showSuccess('Tạo hóa đơn thành công!');
        this.loadBookings(); // Refresh bookings to show the new invoice
        // Navigate to invoice detail page
        this.router.navigate(['/invoices', invoice.id]);
      },
      error: (err) => {
        this.toast.showError(err.error?.message || 'Lỗi khi tạo hóa đơn');
      }
    });
  }
}
