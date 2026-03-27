import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { DOCUMENT } from '@angular/common';
import { InvoiceService } from '../../services/invoice.service';
import { InvoicePdfViewerComponent } from '../../components/invoice-pdf-viewer/invoice-pdf-viewer.component';

@Component({
  selector: 'app-invoice-pdf-page',
  standalone: true,
  imports: [CommonModule, InvoicePdfViewerComponent],
  template: `
    <div class="pdf-page-container">
      <div class="pdf-header">
        <div class="header-actions">
          <button class="btn btn-secondary" (click)="printInvoice()">
            <i class="fas fa-print"></i> In PDF
          </button>
          <button class="btn btn-primary" (click)="downloadPdf()">
            <i class="fas fa-download"></i> Tải PDF
          </button>
          <button class="btn btn-outline" (click)="goBack()">
            <i class="fas fa-arrow-left"></i> Quay lại
          </button>
        </div>
      </div>
      
      <div class="pdf-content">
        <app-invoice-pdf-viewer [invoiceData]="invoiceData"></app-invoice-pdf-viewer>
      </div>
    </div>
  `,
  styles: [`
    .pdf-page-container {
      min-height: 100vh;
      background: #f5f5f5;
    }

    .pdf-header {
      background: white;
      padding: 15px 20px;
      border-bottom: 1px solid #e5e7eb;
      position: sticky;
      top: 0;
      z-index: 100;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .header-actions {
      display: flex;
      gap: 10px;
      justify-content: flex-end;
      max-width: 1200px;
      margin: 0 auto;
    }

    .btn {
      padding: 8px 16px;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 500;
      display: flex;
      align-items: center;
      gap: 6px;
      transition: all 0.2s;
    }

    .btn:hover {
      transform: translateY(-1px);
      box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    }

    .btn-primary {
      background: #2563eb;
      color: white;
    }

    .btn-primary:hover {
      background: #1d4ed8;
    }

    .btn-secondary {
      background: #6b7280;
      color: white;
    }

    .btn-secondary:hover {
      background: #4b5563;
    }

    .btn-outline {
      background: white;
      color: #6b7280;
      border: 1px solid #d1d5db;
    }

    .btn-outline:hover {
      background: #f9fafb;
      border-color: #9ca3af;
    }

    .pdf-content {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }

    @media print {
      .pdf-header {
        display: none;
      }
      
      .pdf-content {
        padding: 0;
      }
    }
  `]
})
export class InvoicePdfPageComponent {
  invoiceData: any = null;

  constructor(
    private route: ActivatedRoute,
    private invoiceService: InvoiceService,
    @Inject(DOCUMENT) private document: Document
  ) {}

  ngOnInit(): void {
    const invoiceId = this.route.snapshot.paramMap.get('id');
    if (invoiceId) {
      this.loadInvoiceData(invoiceId);
    }
  }

  loadInvoiceData(id: string): void {
    console.log('Loading invoice data for ID:', id);
    
    this.invoiceService.getInvoiceById(id).subscribe({
      next: (invoice) => {
        console.log('Invoice data loaded:', invoice);
        
        // Enrich invoice data with additional details
        this.invoiceData = {
          ...invoice,
          // Add mock data for missing fields
          customerPhone: '0912345678',
          unitPrice: (invoice as any).trip?.price || 350000,
          seats: [
            { seatNumber: 'A01', type: 'Giường nằm' },
            { seatNumber: 'A02', type: 'Giường nằm' }
          ],
          pickupPoint: { name: 'Bến xe Miền Tây' },
          dropoffPoint: { name: 'Bến xe Liên tỉnh Đà Lạt' },
          bus: {
            type: 'Giường nằm 44 chỗ',
            licensePlate: '47A-12345'
          },
          paymentMethod: 'vnpay',
          // Add trip details if available
          trip: (invoice as any).trip || {
            origin: 'Sài Gòn',
            destination: 'Đà Lạt',
            routeName: 'Sài Gòn - Đà Lạt',
            price: 350000,
            departureTime: invoice.createdAt,
            arrivalTime: new Date(new Date(invoice.createdAt).getTime() + 7 * 60 * 60 * 1000)
          }
        };
        
        console.log('Final invoice data:', this.invoiceData);
      },
      error: (err) => {
        console.error('Failed to load invoice data:', err);
        alert('Không thể tải thông tin hóa đơn. Vui lòng thử lại.');
      }
    });
  }

  downloadPdf(): void {
    const invoiceId = this.route.snapshot.paramMap.get('id');
    if (invoiceId) {
      console.log('Downloading PDF for invoice ID:', invoiceId);
      
      this.invoiceService.exportPdf(invoiceId).subscribe({
        next: () => {
          console.log('PDF downloaded successfully');
          alert('Tải PDF thành công!');
        },
        error: (err) => {
          console.error('Error downloading PDF:', err);
          alert('Lỗi khi tải PDF. Vui lòng thử lại.');
        }
      });
    }
  }

  printInvoice(): void {
    (this.document.defaultView as any).print();
  }

  goBack(): void {
    window.history.back();
  }
}
