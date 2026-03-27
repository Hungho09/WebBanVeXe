import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-invoice-pdf-viewer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="invoice-pdf-container" *ngIf="invoiceData">
      <div class="invoice-header">
        <div class="company-info">
          <h1>VÉ XE ONLINE</h1>
          <p>Đặt vé nhanh chóng - An toàn tuyệt đối</p>
          <p>📞 Hotline: 1900-1234 | ✉️ Email: support@vexeonline.vn</p>
        </div>
        <div class="invoice-number">
          <h2>HÓA ĐƠN</h2>
          <p class="invoice-id">#{{ invoiceData.invoiceNumber }}</p>
          <p class="invoice-date">Ngày: {{ invoiceData.createdAt | date:'dd/MM/yyyy HH:mm' }}</p>
        </div>
      </div>

      <div class="invoice-section">
        <h3>📋 Thông tin khách hàng</h3>
        <div class="info-grid">
          <div class="info-item">
            <strong>Họ tên:</strong> {{ invoiceData.customerName }}
          </div>
          <div class="info-item">
            <strong>Email:</strong> {{ invoiceData.customerEmail }}
          </div>
          <div class="info-item">
            <strong>Số điện thoại:</strong> {{ invoiceData.customerPhone || 'Chưa cung cấp' }}
          </div>
        </div>
      </div>

      <div class="invoice-section">
        <h3>🚌 Thông tin chuyến đi</h3>
        <div class="trip-info">
          <div class="route">
            <div class="departure">
              <strong>Từ:</strong> {{ invoiceData.trip?.origin || invoiceData.origin }}
              <p>{{ invoiceData.departureTime | date:'dd/MM/yyyy HH:mm' }}</p>
            </div>
            <div class="arrow">→</div>
            <div class="arrival">
              <strong>Đến:</strong> {{ invoiceData.trip?.destination || invoiceData.destination }}
              <p>{{ invoiceData.arrivalTime | date:'dd/MM/yyyy HH:mm' }}</p>
            </div>
          </div>
          <div class="trip-details">
            <p><strong>Tuyến đường:</strong> {{ invoiceData.trip?.routeName || invoiceData.routeName }}</p>
            <p><strong>Loại xe:</strong> {{ invoiceData.bus?.type || invoiceData.busType || 'Giường nằm 44 chỗ' }}</p>
            <p><strong>Biển số:</strong> {{ invoiceData.bus?.licensePlate || invoiceData.licensePlate || '47A-12345' }}</p>
          </div>
        </div>
      </div>

      <div class="invoice-section">
        <h3>🪑 Thông tin ghế ngồi</h3>
        <div class="seats-info">
          <div class="seats-list">
            <span class="seat-badge" *ngFor="let seat of invoiceData.seats">
              {{ seat.seatNumber }}
            </span>
          </div>
          <div class="seat-details">
            <p><strong>Số lượng:</strong> {{ invoiceData.seats?.length || 1 }} ghế</p>
            <p><strong>Điểm đón:</strong> {{ invoiceData.pickupPoint?.name || invoiceData.pickupPoint }}</p>
            <p><strong>Điểm trả:</strong> {{ invoiceData.dropoffPoint?.name || invoiceData.dropoffPoint }}</p>
          </div>
        </div>
      </div>

      <div class="invoice-section">
        <h3>💰 Chi tiết thanh toán</h3>
        <div class="payment-details">
          <div class="price-row">
            <span>Giá vé gốc:</span>
            <span>{{ invoiceData.unitPrice || invoiceData.trip?.price | number }} VNĐ</span>
          </div>
          <div class="price-row">
            <span>Số lượng ghế:</span>
            <span>{{ invoiceData.seats?.length || 1 }}</span>
          </div>
          <div class="price-row subtotal">
            <span>Thành tiền:</span>
            <span>{{ (invoiceData.unitPrice || invoiceData.trip?.price) * (invoiceData.seats?.length || 1) | number }} VNĐ</span>
          </div>
          <div class="price-row total">
            <span><strong>TỔNG CỘNG:</strong></span>
            <span><strong>{{ invoiceData.totalAmount | number }} VNĐ</strong></span>
          </div>
          <div class="payment-method">
            <p><strong>Phương thức thanh toán:</strong> {{ getPaymentMethod(invoiceData.paymentMethod) }}</p>
            <p><strong>Trạng thái:</strong> <span class="status-paid">✅ Đã thanh toán</span></p>
          </div>
        </div>
      </div>

      <div class="invoice-footer">
        <div class="notes">
          <h4>📝 Lưu ý:</h4>
          <ul>
            <li>Vui lòng có mặt tại điểm đón trước 15 phút</li>
            <li>Đ mang theo CMND/CCCD khi lên xe</li>
            <li>Không hoàn vé trong vòng 2 giờ trước departure</li>
            <li>Liên hệ hotline nếu cần hỗ trợ</li>
          </ul>
        </div>
        <div class="signature">
          <p>Khách hàng xác nhận</p>
          <div class="signature-line"></div>
          <p>(Ký và ghi rõ họ tên)</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .invoice-pdf-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px;
      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
      background: white;
      color: #333;
    }

    .invoice-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      border-bottom: 3px solid #2563eb;
      padding-bottom: 20px;
      margin-bottom: 30px;
    }

    .company-info h1 {
      color: #2563eb;
      margin: 0 0 5px 0;
      font-size: 28px;
    }

    .company-info p {
      margin: 3px 0;
      color: #666;
      font-size: 14px;
    }

    .invoice-number {
      text-align: right;
    }

    .invoice-number h2 {
      color: #1f2937;
      margin: 0 0 5px 0;
      font-size: 24px;
    }

    .invoice-id {
      font-size: 18px;
      font-weight: bold;
      color: #2563eb;
      margin: 5px 0;
    }

    .invoice-date {
      font-size: 14px;
      color: #666;
      margin: 0;
    }

    .invoice-section {
      margin-bottom: 25px;
    }

    .invoice-section h3 {
      color: #2563eb;
      border-bottom: 2px solid #e5e7eb;
      padding-bottom: 8px;
      margin-bottom: 15px;
      font-size: 18px;
    }

    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 15px;
    }

    .info-item {
      padding: 10px;
      background: #f8f9fa;
      border-radius: 6px;
      border-left: 4px solid #2563eb;
    }

    .trip-info .route {
      display: flex;
      align-items: center;
      justify-content: space-between;
      background: #f8f9fa;
      padding: 20px;
      border-radius: 8px;
      margin-bottom: 15px;
    }

    .departure, .arrival {
      flex: 1;
    }

    .departure strong, .arrival strong {
      color: #2563eb;
      font-size: 16px;
    }

    .arrow {
      font-size: 24px;
      color: #2563eb;
      font-weight: bold;
      margin: 0 20px;
    }

    .trip-details {
      background: #f8f9fa;
      padding: 15px;
      border-radius: 6px;
    }

    .trip-details p {
      margin: 5px 0;
    }

    .seats-info {
      background: #f8f9fa;
      padding: 20px;
      border-radius: 8px;
    }

    .seats-list {
      margin-bottom: 15px;
    }

    .seat-badge {
      display: inline-block;
      background: #2563eb;
      color: white;
      padding: 8px 12px;
      margin: 5px;
      border-radius: 20px;
      font-weight: bold;
      font-size: 14px;
    }

    .seat-details p {
      margin: 5px 0;
    }

    .payment-details {
      background: #f8f9fa;
      padding: 20px;
      border-radius: 8px;
    }

    .price-row {
      display: flex;
      justify-content: space-between;
      padding: 8px 0;
      border-bottom: 1px solid #e5e7eb;
    }

    .price-row:last-child {
      border-bottom: none;
    }

    .subtotal {
      font-weight: 600;
      color: #4b5563;
    }

    .total {
      font-size: 18px;
      font-weight: bold;
      color: #2563eb;
      border-top: 2px solid #2563eb;
      padding-top: 10px;
      margin-top: 10px;
    }

    .payment-method {
      margin-top: 15px;
      padding-top: 15px;
      border-top: 1px solid #e5e7eb;
    }

    .status-paid {
      color: #10b981;
      font-weight: bold;
    }

    .invoice-footer {
      display: flex;
      justify-content: space-between;
      margin-top: 40px;
      padding-top: 20px;
      border-top: 2px solid #e5e7eb;
    }

    .notes {
      flex: 2;
    }

    .notes h4 {
      color: #2563eb;
      margin-bottom: 10px;
    }

    .notes ul {
      margin: 0;
      padding-left: 20px;
    }

    .notes li {
      margin: 5px 0;
      font-size: 14px;
      color: #666;
    }

    .signature {
      flex: 1;
      text-align: center;
    }

    .signature p {
      margin: 5px 0;
      font-size: 14px;
    }

    .signature-line {
      width: 200px;
      height: 2px;
      border-bottom: 2px solid #333;
      margin: 20px auto 5px;
    }

    @media print {
      .invoice-pdf-container {
        margin: 0;
        padding: 10px;
      }
      
      .invoice-header {
        flex-direction: column;
        text-align: center;
      }
      
      .invoice-footer {
        flex-direction: column;
      }
    }
  `]
})
export class InvoicePdfViewerComponent {
  @Input() invoiceData: any = null;

  getPaymentMethod(method?: string): string {
    switch (method) {
      case 'vnpay': return 'Thanh toán trực tuyến (VNPay)';
      case 'momo': return 'Ví MoMo';
      case 'cash': return 'Thanh toán khi lên xe';
      default: return 'Thanh toán trực tuyến';
    }
  }
}
