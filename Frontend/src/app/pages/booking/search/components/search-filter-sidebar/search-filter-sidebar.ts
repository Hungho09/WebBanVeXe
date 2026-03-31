import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-filter-sidebar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-filter-sidebar.html',
  styleUrl: './search-filter-sidebar.css'
})
export class SearchFilterSidebarComponent {
  @Input() companies: string[] = [];
  @Input() pickupPoints: string[] = [];
  @Input() dropoffPoints: string[] = [];
  
  @Output() sortChange = new EventEmitter<string>();
  @Output() filterChange = new EventEmitter<any>();

  sortBy: string = 'default';
  
  // Filter states
  selectedCompanies: { [key: string]: boolean } = {};
  priceRange: number = 1000000; // max 1M for demo
  selectedHours: string[] = [];

  sortOptions = [
    { label: 'Mặc định', value: 'default' },
    { label: 'Giờ đi sớm nhất', value: 'early' },
    { label: 'Giờ đi muộn nhất', value: 'late' },
    { label: 'Đánh giá cao nhất', value: 'rating' },
    { label: 'Giá tăng dần', value: 'priceAsc' },
    { label: 'Giá giảm dần', value: 'priceDesc' }
  ];

  timeFilters = [
    { label: 'Sáng sớm 00:00 - 06:00', value: 'dawn' },
    { label: 'Buổi sáng 06:00 - 12:00', value: 'morning' },
    { label: 'Buổi chiều 12:00 - 18:00', value: 'afternoon' },
    { label: 'Buổi tối 18:00 - 24:00', value: 'evening' }
  ];

  onSortSelect(val: string) {
    this.sortBy = val;
    this.sortChange.emit(this.sortBy);
  }

  toggleCompany(name: string) {
    this.selectedCompanies[name] = !this.selectedCompanies[name];
    this.emitFilters();
  }

  onPriceChange() {
    this.emitFilters();
  }

  clearFilters() {
    this.selectedCompanies = {};
    this.priceRange = 1000000;
    this.emitFilters();
  }

  private emitFilters() {
    this.filterChange.emit({
      companies: Object.keys(this.selectedCompanies).filter(k => this.selectedCompanies[k]),
      maxPrice: this.priceRange
    });
  }
}
