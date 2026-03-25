import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Invoice } from '../../models/invoice.model';
import { InvoiceService } from '../../services/invoice.service';
import { InvoiceFormatPipe } from '../../pipes/invoice-format.pipe';

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  imports: [CommonModule, RouterLink, InvoiceFormatPipe],
  templateUrl: './invoice-list.component.html',
  styleUrl: './invoice-list.component.css'
})
export class InvoiceListComponent implements OnInit {
  invoices: Invoice[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private readonly invoiceService: InvoiceService) {}

  ngOnInit(): void {
    this.loadInvoices();
  }

  private loadInvoices(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.invoiceService.getAll().subscribe({
      next: (items) => {
        this.invoices = items;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Không tải được danh sách hóa đơn.';
        this.isLoading = false;
      }
    });
  }
}
