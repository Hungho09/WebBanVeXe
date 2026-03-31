import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { Trip } from '../../../../../services/trip.service';

@Component({
  selector: 'app-search-result-card',
  standalone: true,
  imports: [CommonModule, DecimalPipe],
  templateUrl: './search-result-card.html',
  styleUrl: './search-result-card.css'
})
export class SearchResultCardComponent {
  @Input() trip!: Trip;
  @Output() selectTrip = new EventEmitter<string>();

  formatTime(iso: string): string {
    const d = new Date(iso);
    return d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
  }

  getDuration(start: string, end: string): string {
    const s = new Date(start);
    const e = new Date(end);
    const diff = e.getTime() - s.getTime();
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    return `${hours}h ${minutes}m`;
  }

  onSelect() {
    this.selectTrip.emit(this.trip.id);
  }
}

