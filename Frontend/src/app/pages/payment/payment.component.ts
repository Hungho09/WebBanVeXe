import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PaymentService } from '../../services/payment.service';
import { PaymentResultDto } from '../../models/payment.model';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
  bookingId: string = '';
  amount: number = 0;
  paymentMethod: string = 'VNPAY';
  
  isProcessing: boolean = false;
  paymentResult: PaymentResultDto | null = null;
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private paymentService: PaymentService
  ) {}

  ngOnInit(): void {
    // Thông thường bookingId và amount sẽ được truyền qua state hoặc query params
    this.route.queryParams.subscribe(params => {
      if (params['bookingId']) {
        this.bookingId = params['bookingId'];
        this.amount = params['amount'] ? Number(params['amount']) : 500000; // Giá trị mặc định mô phỏng
      }
    });
  }

  processPayment() {
    if (!this.bookingId) {
      this.errorMessage = 'Mã đặt chỗ không hợp lệ!';
      return;
    }

    this.isProcessing = true;
    this.errorMessage = '';

    const paymentReq = {
      bookingId: this.bookingId,
      paymentMethod: this.paymentMethod
    };

    this.paymentService.processPayment(paymentReq).subscribe({
      next: (result) => {
        this.paymentResult = result;
        this.isProcessing = false;
      },
      error: (err) => {
        console.error('Lỗi khi xử lý thanh toán', err);
        this.errorMessage = err.error?.message || 'Thanh toán thất bại.';
        this.isProcessing = false;
      }
    });
  }

  goBackToHome() {
    this.router.navigate(['/']);
  }
}
