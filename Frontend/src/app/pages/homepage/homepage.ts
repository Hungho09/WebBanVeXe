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
        DecimalPipe,
        RouterLink,
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

    tripSearchAttempted = false;
    tripSearchLoading = false;
    tripSearchError: string | null = null;
    tripSearchDateNote: string | null = null;
    tripSearchResults: Trip[] = [];

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

    onTripSearch(payload: { origin: string; destination: string; departureDate: Date }): void {
        this.tripSearchAttempted = true;
        this.tripSearchDateNote = null;
        if (!payload.origin || !payload.destination) {
            this.tripSearchError = 'Vui lòng chọn điểm đi và điểm đến.';
            this.tripSearchResults = [];
            this.tripSearchLoading = false;
            return;
        }

        this.tripSearchLoading = true;
        this.tripSearchError = null;
        this.tripSearchResults = [];

        this.tripService.getTrips().subscribe({
            next: (trips) => {
                const byRoute = (t: Trip) => this.tripMatchesRouteByPlace(t, payload.origin, payload.destination);
                const byDay = (t: Trip) => byRoute(t) && this.sameLocalDay(new Date(t.departureTime), payload.departureDate);
                let filtered = trips.filter(byDay);
                if (filtered.length === 0) {
                    filtered = trips.filter(byRoute);
                    if (filtered.length > 0) {
                        this.tripSearchDateNote =
                            'Không có chuyến đúng ngày đã chọn; đang hiển thị các chuyến cùng tuyến.';
                    }
                }
                this.tripSearchResults = filtered;
                this.tripSearchLoading = false;
            },
            error: () => {
                this.tripSearchError = 'Không tải được danh sách chuyến. Hãy chạy API và kiểm tra proxy /api.';
                this.tripSearchLoading = false;
            },
        });
    }

    private locationKeywords(label: string): string[] {
        const s = label.trim().toLowerCase();
        if (!s) return [];
        if (/hồ chí minh|tp\.?\s*hcm|sài gòn/.test(s)) return ['tp.hcm', 'hồ chí minh', 'tpchm'];
        if (/đà lạt|lâm đồng/.test(s)) return ['đà lạt', 'lâm đồng'];
        if (/hà nội/.test(s)) return ['hà nội'];
        if (/hải phòng/.test(s)) return ['hải phòng'];
        return [s];
    }

    private tripMatchesRouteByPlace(trip: Trip, origin: string, dest: string): boolean {
        const rn = trip.routeName.toLowerCase();
        return (
            this.locationKeywords(origin).some((k) => rn.includes(k)) &&
            this.locationKeywords(dest).some((k) => rn.includes(k))
        );
    }

    private sameLocalDay(dep: Date, day: Date): boolean {
        return (
            dep.getFullYear() === day.getFullYear() &&
            dep.getMonth() === day.getMonth() &&
            dep.getDate() === day.getDate()
        );
    }

    formatTripTime(iso: string): string {
        const d = new Date(iso);
        return d.toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' });
    }
}
