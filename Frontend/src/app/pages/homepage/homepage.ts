import { Component, HostListener, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CmsService, CmsConfig } from '../../services/cms.service';
import { TripService, Trip } from '../../services/trip.service';

// Shared Layout Components
import { NavbarComponent } from '../../shared/layout/navbar/navbar';
import { FooterComponent } from '../../shared/layout/footer/footer';

// Homepage Section Components
import { HeroSearchComponent } from './sections/hero-search/hero-search';
import { PopularRoutesComponent } from './sections/popular-routes/popular-routes';
import { OffersNewsComponent } from './sections/offers-news/offers-news';
import { TestimonialsComponent } from './sections/testimonials/testimonials';

/**
 * Homepage — Trang chủ public
 *
 * Chỉ chứa logic điều phối cấp cao:
 * - Trạng thái scroll header
 * - Background rotation
 * - Dữ liệu CMS (truyền xuống section components)
 * - Auth state
 */
@Component({
    selector: 'app-homepage',
    standalone: true,
    imports: [
        CommonModule,
        NavbarComponent,
        FooterComponent,
        HeroSearchComponent,
        PopularRoutesComponent,
        OffersNewsComponent,
        TestimonialsComponent,
    ],
    templateUrl: './homepage.html',
    styleUrl: './homepage.css',
})
export class Homepage implements OnInit, OnDestroy {

    // ── Scroll state (cho navbar glass effect) ────────────────
    isScrolled = false;

    @HostListener('window:scroll', [])
    onWindowScroll() {
        this.isScrolled = window.scrollY > 50;
    }

    // ── Auth ──────────────────────────────────────────────────
    currentUser: any = null;

    // ── Background Rotation ───────────────────────────────────
    private heroBgImages = [
        'bg.png','bg2.png','bg3.png','bg4.png','bg5.png',
        'bg7.png','bg8.png','bg9.png','bg10.png'
    ];
    private currentBgIndex = 0;
    private bgInterval: any;

    get bgImageUrl(): string {
        return `/assets/${this.heroBgImages[this.currentBgIndex]}`;
    }

    // ── CMS Data ──────────────────────────────────────────────
    cmsConfig: CmsConfig | null = null;
    tripSearchError: string | null = null;

    constructor(
        public authService: AuthService,
        private router: Router,
        private cmsService: CmsService,
        private tripService: TripService,
    ) {
        this.currentUser = this.authService.getUser();
        // Load local cache initially
        this.cmsConfig = this.cmsService.getConfig();
    }

    ngOnInit() {
        // Fetch from server
        this.cmsService.getRemoteConfig().subscribe(config => {
            this.cmsConfig = config;
        });

        this.bgInterval = setInterval(() => {
            this.currentBgIndex = (this.currentBgIndex + 1) % this.heroBgImages.length;
        }, 5000);
    }

    ngOnDestroy() {
        if (this.bgInterval) clearInterval(this.bgInterval);
    }

    // ── Auth Actions ──────────────────────────────────────────
    logout() {
        this.authService.logout();
        this.currentUser = null;
        window.location.reload();
    }

    onTripSearch(payload: any): void {
        // Hero component now handles navigation directly.
        // We can use this for tracking or additional logic.
    }


    formatTripTime(iso: string): string {
        const d = new Date(iso);
        return d.toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' });
    }
}
