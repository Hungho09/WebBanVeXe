import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-header-mini',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-header-mini.html',
  styleUrl: './search-header-mini.css'
})
export class SearchHeaderMiniComponent {
  @Input() origin: string = '';
  @Input() destination: string = '';
  @Input() date: string = '';
  
  @Output() searchChange = new EventEmitter<any>();

  onResubmit() {
    this.searchChange.emit({
      origin: this.origin,
      destination: this.destination,
      date: this.date
    });
  }
}
