import { Component, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

/**
 * NavbarComponent — Public site navbar (trang chủ)
 * 
 * Nhận: isScrolled (từ host scroll listener)
 * Nhận: currentUser (từ AuthService)
 * Phát: logoutEvent khi user click đăng xuất
 *
 * Sử dụng: <app-navbar [isScrolled]="isScrolled" [currentUser]="currentUser" (logoutEvent)="logout()">
 */
@Component({
    selector: 'app-navbar',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './navbar.html',
    styleUrl: './navbar.css',
})
export class NavbarComponent {
    @Input() isScrolled: boolean = false;
    @Input() currentUser: any = null;
    @Output() logoutEvent = new EventEmitter<void>();

    constructor(
        public authService: AuthService,
        private router: Router,
    ) {}

    navigateTo(url: string) {
        this.router.navigateByUrl(url);
    }

    onLogout() {
        this.logoutEvent.emit();
    }
}
