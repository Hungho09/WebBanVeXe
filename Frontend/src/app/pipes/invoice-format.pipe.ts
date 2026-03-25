import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'invoiceFormat',
  standalone: true
})
export class InvoiceFormatPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) return '';
    const normalized = value.trim().toUpperCase();
    return /^INV-\d{8}-\d{4}$/.test(normalized) ? normalized : value;
  }
}
