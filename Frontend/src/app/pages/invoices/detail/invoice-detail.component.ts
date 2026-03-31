import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Invoice } from '../../../models/invoice.model';
import { InvoiceService } from '../../../services/invoice.service';
import { InvoiceFormatPipe } from '../../../pipes/invoice-format.pipe';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, InvoiceFormatPipe],
  templateUrl: './invoice-detail.component.html',
  styleUrl: './invoice-detail.component.css'
})
export class InvoiceDetailComponent implements OnInit {
  invoice: Invoice | null = null;
  isLoading = false;
  errorMessage = '';
  isDownloading = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly invoiceService: InvoiceService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.errorMessage = 'Thiếu mã hóa đơn.';
      return;
    }

    this.isLoading = true;
    this.invoiceService.getInvoiceById(id).subscribe({
      next: (item) => {
        this.invoice = item;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Không tìm thấy hóa đơn.';
        this.isLoading = false;
      }
    });
  }

  downloadJson(): void {
    if (!this.invoice) return;
    
    this.isDownloading = true;
    this.invoiceService.exportJson(this.invoice.id).subscribe({
      next: () => {
        
        this.isDownloading = false;
      },
      error: (err) => {
        console.error('Error downloading JSON:', err);
        this.isDownloading = false;
        alert('Lỗi khi tải file JSON. Vui lòng thử lại.');
      }
    });
  }

  downloadPdf(): void {
    if (!this.invoice) return;
    
    this.isDownloading = true;
    this.invoiceService.exportPdf(this.invoice.id).subscribe({
      next: () => {
        
        this.isDownloading = false;
      },
      error: (err) => {
        console.error('Error downloading PDF:', err);
        this.isDownloading = false;
        alert('Lỗi khi tải file PDF. Vui lòng thử lại.');
      }
    });
  }
}
