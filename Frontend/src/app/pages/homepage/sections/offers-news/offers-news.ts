import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * OffersNewsComponent — Ưu đãi + Thông báo mới
 * Inputs: promos, notices — từ CmsConfig
 */
@Component({
    selector: 'app-offers-news',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './offers-news.html',
    styleUrl: './offers-news.css',
})
export class OffersNewsComponent {
    @Input() promos: any[] = [];
    @Input() notices: any[] = [];
}
