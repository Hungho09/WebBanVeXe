import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * TestimonialsComponent — Đánh giá khách hàng
 * Input: testimonials — từ CmsConfig.testimonials
 */
@Component({
    selector: 'app-testimonials',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './testimonials.html',
    styleUrl: './testimonials.css',
})
export class TestimonialsComponent {
    @Input() testimonials: any[] = [];
    readonly defaultAvatar = '/assets/bus.png';

    // Drag-to-scroll logic for Desktop
    startDragging(e: MouseEvent, el: HTMLDivElement) {
        const startX = e.pageX - el.offsetLeft;
        const scrollLeft = el.scrollLeft;

        const onMouseMove = (moveEvent: MouseEvent) => {
            const x = moveEvent.pageX - el.offsetLeft;
            const walk = (x - startX) * 2; // Scroll speed
            el.scrollLeft = scrollLeft - walk;
        };

        const onMouseUp = () => {
            window.removeEventListener('mousemove', onMouseMove);
            window.removeEventListener('mouseup', onMouseUp);
        };

        window.addEventListener('mousemove', onMouseMove);
        window.addEventListener('mouseup', onMouseUp);
    }
}
