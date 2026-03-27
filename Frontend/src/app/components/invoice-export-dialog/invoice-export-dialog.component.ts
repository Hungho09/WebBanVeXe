import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-invoice-export-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dialog-overlay" *ngIf="visible">
      <div class="dialog-box">
        <div class="dialog-header">
          <h3>🎉 Thanh toán thành công!</h3>
        </div>
        
        <div class="dialog-body">
          <p>Bạn có muốn xuất file PDF hóa đơn không?</p>
          <div class="invoice-preview" *ngIf="invoiceInfo">
            <p><strong>Mã hóa đơn:</strong> {{ invoiceInfo.invoiceNumber }}</p>
            <p><strong>Số tiền:</strong> {{ invoiceInfo.totalAmount | number }} VNĐ</p>
          </div>
        </div>
        
        <div class="dialog-footer">
          <button class="btn btn-secondary" (click)="onDecline()">
            <i class="fas fa-home"></i> Về trang chủ
          </button>
          <button class="btn btn-primary" (click)="onAccept()" [disabled]="isExporting">
            <i class="fas fa-file-pdf" *ngIf="!isExporting"></i>
            <i class="fas fa-spinner fa-spin" *ngIf="isExporting"></i>
            {{ isExporting ? 'Đang xuất...' : 'Xuất PDF' }}
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dialog-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 1000;
    }
    
    .dialog-box {
      background: white;
      border-radius: 8px;
      padding: 0;
      max-width: 400px;
      width: 90%;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
      animation: slideIn 0.3s ease-out;
    }
    
    @keyframes slideIn {
      from {
        opacity: 0;
        transform: translateY(-20px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }
    
    .dialog-header {
      background: linear-gradient(135deg, #28a745, #20c997);
      color: white;
      padding: 20px;
      border-radius: 8px 8px 0 0;
      text-align: center;
    }
    
    .dialog-header h3 {
      margin: 0;
      font-size: 18px;
      font-weight: 600;
    }
    
    .dialog-body {
      padding: 20px;
      text-align: center;
    }
    
    .dialog-body p {
      margin: 0 0 15px 0;
      font-size: 16px;
      color: #333;
    }
    
    .invoice-preview {
      background: #f8f9fa;
      padding: 15px;
      border-radius: 6px;
      margin: 15px 0;
      text-align: left;
    }
    
    .invoice-preview p {
      margin: 5px 0;
      font-size: 14px;
    }
    
    .dialog-footer {
      padding: 20px;
      border-top: 1px solid #eee;
      display: flex;
      gap: 10px;
      justify-content: center;
    }
    
    .btn {
      padding: 10px 20px;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 500;
      display: flex;
      align-items: center;
      gap: 8px;
      transition: all 0.2s;
    }
    
    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    
    .btn-secondary {
      background: #6c757d;
      color: white;
    }
    
    .btn-secondary:hover:not(:disabled) {
      background: #5a6268;
    }
    
    .btn-primary {
      background: #007bff;
      color: white;
    }
    
    .btn-primary:hover:not(:disabled) {
      background: #0056b3;
    }
    
    .fa-spinner {
      animation: spin 1s linear infinite;
    }
    
    @keyframes spin {
      from { transform: rotate(0deg); }
      to { transform: rotate(360deg); }
    }
  `]
})
export class InvoiceExportDialogComponent {
  @Input() visible = false;
  @Input() invoiceInfo: any = null;
  @Output() accept = new EventEmitter<void>();
  @Output() decline = new EventEmitter<void>();
  
  isExporting = false;

  onAccept(): void {
    this.isExporting = true;
    this.accept.emit();
  }

  onDecline(): void {
    this.decline.emit();
  }
}
