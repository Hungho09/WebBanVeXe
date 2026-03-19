import { Component, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { RouterLink, Router } from '@angular/router'; // Thêm Router
import { CommonModule } from '@angular/common';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-homepage',
    standalone: true,
    imports: [RouterLink, CommonModule, ScrollingModule, FormsModule],
    templateUrl: './homepage.html',
    styleUrl: './homepage.css',
})
export class Homepage implements AfterViewInit {
    // Auth State
    currentUser: any = null;

    constructor(
        public authService: AuthService,
        private router: Router // Inject Router
    ) {
        this.currentUser = this.authService.getUser();
    }

    navigateTo(url: string) {
        this.router.navigateByUrl(url);
    }
    
    logout() {
        this.authService.logout();
        this.currentUser = null;
        window.location.reload();
    }
    // Custom DatePicker State
    showCustomCalendar = false;
    isLunarMode = false;
    calendarLang: 'en' | 'vi' = 'vi';
    viewDate: Date = new Date(); // Tháng đang hiển thị trên lịch
    calendarDays: any[] = [];
    weekDays = {
        vi: ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'],
        en: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
    };
    monthNames = {
        vi: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
        en: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']
    };
    showingHotline = false;

    tripType: 'oneWay' | 'roundTrip' = 'oneWay';

    provinces: string[] = [
        'An Giang', 'Bà Rịa - Vũng Tàu', 'Bạc Liêu', 'Bắc Giang', 'Bắc Kạn', 'Bắc Ninh', 'Bến Tre', 'Bình Dương',
        'Bình Định', 'Bình Phước', 'Bình Thuận', 'Cà Mau', 'Cao Bằng', 'Cần Thơ', 'Đà Nẵng', 'Đắk Lắk', 'Đắk Nông',
        'Điện Biên', 'Đồng Nai', 'Đồng Tháp', 'Gia Lai', 'Hà Giang', 'Hà Nam', 'Hà Nội', 'Hà Tĩnh', 'Hải Dương',
        'Hải Phòng', 'Hậu Giang', 'Hòa Bình', 'Hồ Chí Minh', 'Hưng Yên', 'Khánh Hòa', 'Kiên Giang', 'Kon Tum',
        'Lai Châu', 'Lạng Sơn', 'Lào Cai', 'Lâm Đồng', 'Long An', 'Nam Định', 'Nghệ An', 'Ninh Bình', 'Ninh Thuận',
        'Phú Thọ', 'Phú Yên', 'Quảng Bình', 'Quảng Nam', 'Quảng Ngãi', 'Quảng Ninh', 'Quảng Trị', 'Sóc Trăng',
        'Sơn La', 'Tây Ninh', 'Thái Bình', 'Thái Nguyên', 'Thanh Hóa', 'Thừa Thiên Huế', 'Tiền Giang', 'Trà Vinh',
        'Tuyên Quang', 'Vĩnh Long', 'Vĩnh Phúc', 'Yên Bái'
    ];

    origin: string = '';
    destination: string = '';

    showOriginDropdown: boolean = false;
    showDestinationDropdown: boolean = false;

    get filteredOrigins(): string[] {
        return this.provinces.filter(p => !this.origin || p.toLowerCase().includes(this.origin.toLowerCase())).filter(p => p !== this.destination);
    }

    get filteredDestinations(): string[] {
        return this.provinces.filter(p => !this.destination || p.toLowerCase().includes(this.destination.toLowerCase())).filter(p => p !== this.origin);
    }

    setTripType(type: 'oneWay' | 'roundTrip', event?: Event) {
        if (event) {
            event.preventDefault(); // Stop radio button triggering click again through bubble
            event.stopPropagation();
        }
        
        if (this.tripType === type) {
            // Toggle to the other type if the active one is clicked again
            this.tripType = (type === 'oneWay') ? 'roundTrip' : 'oneWay';
        } else {
            this.tripType = type;
        }
    }

    selectOrigin(province: string) {
        this.origin = province;
        this.showOriginDropdown = false;
    }

    selectDestination(province: string) {
        this.destination = province;
        this.showDestinationDropdown = false;
    }

    swapLocations() {
        const temp = this.origin;
        this.origin = this.destination;
        this.destination = temp;
    }

    onBlurOrigin() {
        setTimeout(() => this.showOriginDropdown = false, 200);
    }

    onBlurDestination() {
        setTimeout(() => this.showDestinationDropdown = false, 200);
    }

    // Date Slider Properties
    @ViewChild('scrollWrapper', { static: false }) scrollWrapper!: ElementRef;
    datesGroupList: Array<Array<{ date: Date; dayName: string; dateNum: number; dotType: string }>> = [];
    selectedDate: Date = new Date();
    currentMonthYearStr: string = '';

    ngOnInit() {
        this.generateDates(new Date());
    }

    ngAfterViewInit() {
        this.scrollToCenter();
    }

    toggleHotline() {
        this.showingHotline = !this.showingHotline;
    }

    generateDates(centerDate: Date) {
        this.selectedDate = new Date(centerDate.getFullYear(), centerDate.getMonth(), centerDate.getDate()); // Normalize
        const formatter = new Intl.DateTimeFormat('en-US', { month: 'short', year: 'numeric' });
        this.currentMonthYearStr = formatter.format(this.selectedDate);

        this.datesGroupList = [];
        const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        const dotTypes = ['', 'red', 'teal', 'mix', '', 'red', ''];

        // Find the Monday of the current selectedDate's week
        const d = new Date(this.selectedDate);
        const day = d.getDay(); // 0 is Sunday
        const diffToMonday = day === 0 ? -6 : 1 - day;
        const startMonday = new Date(d.getFullYear(), d.getMonth(), d.getDate() + diffToMonday);

        // Generate exactly 7 weeks (49 days) starting from that Monday
        let currentGroup: any[] = [];
        for (let i = 0; i < 49; i++) {
            const date = new Date(startMonday.getFullYear(), startMonday.getMonth(), startMonday.getDate() + i);
            currentGroup.push({
                date: date,
                dayName: dayNames[date.getDay()],
                dateNum: date.getDate(),
                dotType: dotTypes[Math.abs(date.getDate()) % 7]
            });
            if (currentGroup.length === 7) {
                this.datesGroupList.push([...currentGroup]);
                currentGroup = [];
            }
        }

        // Wait for Angular to re-render DOM items before scrolling to the correct week
        setTimeout(() => {
            // Scroll to the left start position (since selected is in first week)
            if (this.scrollWrapper) {
                this.scrollWrapper.nativeElement.scrollLeft = 0;
            }
        }, 50);
    }

    selectDate(dateObj: any) {
        this.selectedDate = dateObj.date;
        const formatter = new Intl.DateTimeFormat('en-US', { month: 'short', year: 'numeric' });
        this.currentMonthYearStr = formatter.format(this.selectedDate);
    }

    // Navigation for Custom Calendar
    prevMonth() {
        this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() - 1, 1);
        this.generateCalendar();
    }

    nextMonth() {
        this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() + 1, 1);
        this.generateCalendar();
    }

    toggleCustomCalendar() {
        this.showCustomCalendar = !this.showCustomCalendar;
        if (this.showCustomCalendar) {
            this.generateCalendar();
        }
    }

    generateCalendar() {
        const year = this.viewDate.getFullYear();
        const month = this.viewDate.getMonth();
        const firstDay = new Date(year, month, 1);
        const lastDay = new Date(year, month + 1, 0);

        // Monday is index 0 in our weekDays array
        let startDay = firstDay.getDay() - 1;
        if (startDay === -1) startDay = 6; // Sunday is index 6

        this.calendarDays = [];

        // Padding previous month's days
        for (let i = 0; i < startDay; i++) {
            this.calendarDays.push({ empty: true });
        }

        // Bảng mốc bắt đầu các tháng âm lịch năm 2026 (Bính Ngọ)
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

        // Current month's days
        for (let i = 1; i <= lastDay.getDate(); i++) {
            const date = new Date(year, month, i);
            const isToday = this.isSameDay(date, new Date());
            const isSelected = this.isSameDay(date, this.selectedDate);

            // Tìm tháng âm lịch phù hợp cho ngày này
            let currentMilestone = lunarMilestones[0];
            for (const m of lunarMilestones) {
                if (date >= m.start) {
                    currentMilestone = m;
                } else {
                    break;
                }
            }

            const diffInTime = date.getTime() - currentMilestone.start.getTime();
            const diffInDays = Math.round(diffInTime / (1000 * 3600 * 24));
            
            const lunarDayNum = diffInDays + 1;
            const lunarMonthNum = currentMilestone.month;

            this.calendarDays.push({
                day: i,
                date: date,
                isToday,
                isSelected,
                lunar: lunarDayNum === 1 ? `${lunarDayNum}/${lunarMonthNum}` : `${lunarDayNum}`
            });
        }
    }

    isSameDay(d1: Date, d2: Date) {
        return d1.getFullYear() === d2.getFullYear() &&
            d1.getMonth() === d2.getMonth() &&
            d1.getDate() === d2.getDate();
    }

    selectCalendarDate(dayObj: any) {
        if (dayObj.empty) return;
        this.selectedDate = dayObj.date;
        this.generateDates(dayObj.date); // Cập nhật slider bên dưới
        this.showCustomCalendar = false;
    }

    onNativeDateChange(event: any) {
        // ... (keep current behavior)
    }

    showDatePicker(dateInput: HTMLInputElement) {
        // Thay vì showPicker mặc định, ta dùng modal tự chế
        this.toggleCustomCalendar();
    }

    // Drag-to-scroll Properties
    isDragging = false;
    startX = 0;
    scrollLeftStart = 0;

    onMouseDown(e: MouseEvent) {
        this.isDragging = true;
        const el = this.scrollWrapper.nativeElement;
        this.startX = e.pageX - el.offsetLeft;
        this.scrollLeftStart = el.scrollLeft;
    }

    onMouseUp() {
        this.isDragging = false;
    }

    onMouseMove(e: MouseEvent) {
        if (!this.isDragging) return;
        e.preventDefault();
        const el = this.scrollWrapper.nativeElement;
        const x = e.pageX - el.offsetLeft;
        const walk = (x - this.startX) * 1.5; // Tùy chỉnh tốc độ kéo (1.5x)
        el.scrollLeft = this.scrollLeftStart - walk;
    }

    scrollToCenter() {
        if (this.scrollWrapper && !this.isDragging) {
            const element = this.scrollWrapper.nativeElement;
            const activeItem = element.querySelector('.active');
            if (activeItem) {
                // Focus active week group precisely
                const weekGroup = activeItem.closest('.week-group');
                if (weekGroup) {
                    element.scrollLeft = weekGroup.offsetLeft;
                } else {
                    const scrollPos = activeItem.offsetLeft - (element.clientWidth / 2) + (activeItem.clientWidth / 2);
                    element.scrollLeft = scrollPos;
                }
            }
        }
    }
}
