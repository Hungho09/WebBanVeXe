import { Component } from '@angular/core';
import { Router } from '@angular/router';

/**
 * FooterComponent — Public site footer (trang chủ)
 * 
 * Sử dụng: <app-footer>
 */
@Component({
    selector: 'app-footer',
    standalone: true,
    imports: [],
    templateUrl: './footer.html',
    styleUrl: './footer.css',
})
export class FooterComponent {
    constructor(private router: Router) {}

    navigateTo(url: string) {
        this.router.navigateByUrl(url);
    }
}
