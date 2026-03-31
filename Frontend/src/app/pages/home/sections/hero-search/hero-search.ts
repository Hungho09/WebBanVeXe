import {
    Component, Input, Output, EventEmitter,
    ElementRef, ViewChild, AfterViewInit, OnInit, OnDestroy
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouteService } from '../../../../services/route.service';

@Component({
    selector: 'app-hero-search',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './hero-search.html',
    styleUrl: './hero-search.css',
})
export class HeroSearchComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() bgImageUrl: string = '';
    @Output() searchEvent = new EventEmitter<any>();

    constructor(private router: Router, private routeService: RouteService) {}

    getBgStyle(): string {
        return `url('${this.bgImageUrl}')`;
    }


    // ── Trip Type ──────────────────────────────────────
    tripType: 'oneWay' | 'roundTrip' = 'oneWay';

    setTripType(type: 'oneWay' | 'roundTrip', event?: Event) {
        if (event) { event.preventDefault(); event.stopPropagation(); }
        this.tripType = (this.tripType === type)
            ? (type === 'oneWay' ? 'roundTrip' : 'oneWay')
            : type;
    }

    // ── Database Locations ───────────────────────────
    dbOrigins: string[] = [];
    dbDestinations: string[] = [];

    origin: string = '';
    destination: string = '';
    showOriginDropdown: boolean = false;
    showDestinationDropdown: boolean = false;

    get filteredOrigins(): string[] {
        return this.dbOrigins
            .filter(p => !this.origin || p.toLowerCase().includes(this.origin.toLowerCase()))
            .filter(p => p !== this.destination);
    }

    get filteredDestinations(): string[] {
        return this.dbDestinations
            .filter(p => !this.destination || p.toLowerCase().includes(this.destination.toLowerCase()))
            .filter(p => p !== this.origin);
    }

    selectOrigin(p: string) { this.origin = p; this.showOriginDropdown = false; }
    selectDestination(p: string) { this.destination = p; this.showDestinationDropdown = false; }
    onBlurOrigin() { setTimeout(() => this.showOriginDropdown = false, 200); }
    onBlurDestination() { setTimeout(() => this.showDestinationDropdown = false, 200); }

    swapLocations() {
        [this.origin, this.destination] = [this.destination, this.origin];
    }

    // ── Custom Calendar ────────────────────────────────
    showCustomCalendar = false;
    isLunarMode = false;
    calendarLang: 'en' | 'vi' = 'vi';
    viewDate: Date = new Date();
    calendarDays: any[] = [];
    weekDays = {
        vi: ['T2','T3','T4','T5','T6','T7','CN'],
        en: ['Mon','Tue','Wed','Thu','Fri','Sat','Sun']
    };
    monthNames = {
        vi: ['Tháng 1','Tháng 2','Tháng 3','Tháng 4','Tháng 5','Tháng 6',
             'Tháng 7','Tháng 8','Tháng 9','Tháng 10','Tháng 11','Tháng 12'],
        en: ['January','February','March','April','May','June',
             'July','August','September','October','November','December']
    };

    toggleCustomCalendar() {
        this.showCustomCalendar = !this.showCustomCalendar;
        if (this.showCustomCalendar) this.generateCalendar();
    }

    prevMonth() {
        this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() - 1, 1);
        this.generateCalendar();
    }

    nextMonth() {
        this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() + 1, 1);
        this.generateCalendar();
    }

    generateCalendar() {
        const y = this.viewDate.getFullYear(), m = this.viewDate.getMonth();
        const first = new Date(y, m, 1), last = new Date(y, m + 1, 0);
        let startDay = first.getDay() - 1;
        if (startDay === -1) startDay = 6;

        this.calendarDays = [];
        for (let i = 0; i < startDay; i++) this.calendarDays.push({ empty: true });

        const lunarMilestones = [
            { start: new Date(2026, 0, 19), month: 12, year: 2025 },
            { start: new Date(2026, 1, 17), month: 1, year: 2026 },
            { start: new Date(2026, 2, 19), month: 2, year: 2026 },
            { start: new Date(2026, 3, 17), month: 3, year: 2026 },
            { start: new Date(2026, 4, 16), month: 4, year: 2026 },
            { start: new Date(2026, 5, 15), month: 5, year: 2026 },
            { start: new Date(2026, 6, 14), month: 6, year: 2026 },
            { start: new Date(2026, 7, 13), month: 7, year: 2026 },
            { start: new Date(2026, 8, 11), month: 8, year: 2026 },
            { start: new Date(2026, 9, 11), month: 9, year: 2026 },
            { start: new Date(2026, 10, 9), month: 10, year: 2026 },
            { start: new Date(2026, 11, 9), month: 11, year: 2026 },
        ];

        for (let i = 1; i <= last.getDate(); i++) {
            const date = new Date(y, m, i);
            const isToday = this.isSameDay(date, new Date());
            const isSelected = this.isSameDay(date, this.selectedDate);

            let milestone = lunarMilestones[0];
            for (const ms of lunarMilestones) {
                if (date >= ms.start) milestone = ms; else break;
            }

            const diff = Math.round((date.getTime() - milestone.start.getTime()) / 86400000);
            const lunarDay = diff + 1;
            this.calendarDays.push({
                day: i, date, isToday, isSelected,
                lunar: lunarDay === 1 ? `${lunarDay}/${milestone.month}` : `${lunarDay}`
            });
        }
    }

    isSameDay(a: Date, b: Date): boolean {
        return a.getFullYear() === b.getFullYear() &&
               a.getMonth() === b.getMonth() &&
               a.getDate() === b.getDate();
    }

    selectCalendarDate(d: any) {
        if (d.empty) return;
        this.selectedDate = d.date;
        this.generateDates(d.date);
        this.showCustomCalendar = false;
    }

    // ── Date Slider ────────────────────────────────────
    @ViewChild('scrollWrapper', { static: false }) scrollWrapper!: ElementRef;
    datesGroupList: Array<Array<{ date: Date; dayName: string; dateNum: number; dotType: string }>> = [];
    selectedDate: Date = new Date();
    currentMonthYearStr: string = '';

    // Drag-to-scroll
    isDragging = false;
    startX = 0;
    scrollLeftStart = 0;

    ngOnInit() { 
        this.generateDates(new Date()); 
        this.loadLocations();
    }

    loadLocations() {
        this.routeService.getLocations().subscribe({
            next: (res: { origins: string[], destinations: string[] }) => {
                this.dbOrigins = res.origins;
                this.dbDestinations = res.destinations;
            },
            error: (err: any) => console.error('Error loading locations from DB:', err)
        });
    }
    ngAfterViewInit() { this.scrollToCenter(); }
    ngOnDestroy() {}

    generateDates(center: Date) {
        this.selectedDate = new Date(center.getFullYear(), center.getMonth(), center.getDate());
        this.currentMonthYearStr = new Intl.DateTimeFormat('en-US', { month: 'short', year: 'numeric' })
            .format(this.selectedDate);

        this.datesGroupList = [];
        const dayNames = ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'];
        const dotTypes = ['','red','teal','mix','','red',''];

        const d = new Date(this.selectedDate);
        const day = d.getDay();
        const diffToMon = day === 0 ? -6 : 1 - day;
        const startMon = new Date(d.getFullYear(), d.getMonth(), d.getDate() + diffToMon);

        let group: any[] = [];
        for (let i = 0; i < 49; i++) {
            const date = new Date(startMon.getFullYear(), startMon.getMonth(), startMon.getDate() + i);
            group.push({ date, dayName: dayNames[date.getDay()], dateNum: date.getDate(), dotType: dotTypes[Math.abs(date.getDate()) % 7] });
            if (group.length === 7) { this.datesGroupList.push([...group]); group = []; }
        }

        setTimeout(() => {
            if (this.scrollWrapper) this.scrollWrapper.nativeElement.scrollLeft = 0;
        }, 50);
    }

    selectDate(ds: any) {
        this.selectedDate = ds.date;
        this.currentMonthYearStr = new Intl.DateTimeFormat('en-US', { month: 'short', year: 'numeric' })
            .format(this.selectedDate);
    }

    onMouseDown(e: MouseEvent) {
        this.isDragging = true;
        const el = this.scrollWrapper.nativeElement;
        this.startX = e.pageX - el.offsetLeft;
        this.scrollLeftStart = el.scrollLeft;
    }

    onMouseUp() { this.isDragging = false; }

    onMouseMove(e: MouseEvent) {
        if (!this.isDragging) return;
        e.preventDefault();
        const el = this.scrollWrapper.nativeElement;
        el.scrollLeft = this.scrollLeftStart - (e.pageX - el.offsetLeft - this.startX) * 1.5;
    }

    scrollToCenter() {
        if (!this.scrollWrapper || this.isDragging) return;
        const el = this.scrollWrapper.nativeElement;
        const active = el.querySelector('.active');
        if (active) {
            const wg = active.closest('.week-group');
            el.scrollLeft = wg ? wg.offsetLeft : active.offsetLeft - el.clientWidth / 2 + active.clientWidth / 2;
        }
    }

    onSearchClick(): void {
        const payload = {
            origin: (this.origin ?? '').trim(),
            destination: (this.destination ?? '').trim(),
            date: this.formatDateForQuery(this.selectedDate)
        };

        // Emit for Homepage (optional if it still wants to do something)
        this.searchEvent.emit(payload);

        // Standard navigation for any page using this component
        this.router.navigate(['/search-results'], {
            queryParams: payload
        });
    }

    private formatDateForQuery(date: Date): string {
        const y = date.getFullYear();
        const m = (date.getMonth() + 1).toString().padStart(2, '0');
        const d = date.getDate().toString().padStart(2, '0');
        return `${y}-${m}-${d}`;
    }
}
