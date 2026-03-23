import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TripService, Trip, Seat } from '../../services/trip.service';
import { BookingService, CreateBookingDto } from '../../services/booking.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './booking.html',
  styleUrl: './booking.css'
})
export class Booking implements OnInit {
  tripId: string | null = null;
  trip: Trip | null = null;
  seats: Seat[] = [];
  selectedSeatIds: string[] = [];
  isLoading = true;
  timerValue = 0; // in seconds
  timerDisplay = '10:00';
  private timerInterval: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tripService: TripService,
    private bookingService: BookingService,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.tripId = this.route.snapshot.paramMap.get('id');
    if (this.tripId) {
      this.loadTripData(this.tripId);
    } else {
      this.toastService.showError('Invalid Trip ID');
      this.router.navigate(['/homepage']);
    }
  }

  ngOnDestroy(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
    // Optional: Unlock all selected seats on destroy
    if (this.selectedSeatIds.length > 0) {
      this.selectedSeatIds.forEach(id => this.bookingService.unlockSeat(id).subscribe());
    }
  }

  loadTripData(id: string): void {
    this.isLoading = true;
    this.tripService.getTrip(id).subscribe({
      next: (trip) => {
        this.trip = trip;
        this.loadSeats(id);
      },
      error: () => {
        this.toastService.showError('Failed to load trip details');
        this.isLoading = false;
      }
    });
  }

  loadSeats(tripId: string): void {
    this.tripService.getSeats(tripId).subscribe({
      next: (seats) => {
        this.seats = seats;
        this.isLoading = false;
      },
      error: () => {
        this.toastService.showError('Failed to load seats');
        this.isLoading = false;
      }
    });
  }

  toggleSeat(seat: Seat): void {
    const isAlreadySelected = this.selectedSeatIds.includes(seat.id);

    if (isAlreadySelected) {
      // Deselect and Unlock
      this.bookingService.unlockSeat(seat.id).subscribe({
        next: () => {
          this.selectedSeatIds = this.selectedSeatIds.filter(id => id !== seat.id);
          if (this.selectedSeatIds.length === 0) {
            this.stopTimer();
          }
          this.loadSeats(this.tripId!); // Refresh to show as available
        },
        error: () => {
          this.toastService.showError('Failed to unlock seat');
        }
      });
    } else {
      // Select and Lock
      if (seat.status !== 'Available') {
        this.toastService.showWarning('This seat is not available');
        return;
      }

      this.bookingService.lockSeat(seat.id).subscribe({
        next: () => {
          this.selectedSeatIds.push(seat.id);
          this.startTimer();
          this.loadSeats(this.tripId!); // Refresh to show as locked
        },
        error: (err) => {
          this.toastService.showError(err.error?.message || 'Failed to lock seat');
        }
      });
    }
  }

  startTimer(): void {
    if (this.timerInterval) return;
    
    this.timerValue = 10 * 60; // 10 minutes
    this.updateTimerDisplay();
    
    this.timerInterval = setInterval(() => {
      this.timerValue--;
      this.updateTimerDisplay();
      
      if (this.timerValue <= 0) {
        this.stopTimer();
        this.handleTimerExpired();
      }
    }, 1000);
  }

  stopTimer(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }
  }

  updateTimerDisplay(): void {
    const minutes = Math.floor(this.timerValue / 60);
    const seconds = this.timerValue % 60;
    this.timerDisplay = `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }

  handleTimerExpired(): void {
    this.toastService.showWarning('Booking session expired. Seats have been released.');
    this.selectedSeatIds = [];
    this.loadSeats(this.tripId!);
  }

  isSelected(seatId: string): boolean {
    return this.selectedSeatIds.includes(seatId);
  }

  get totalAmount(): number {
    return (this.trip?.price || 0) * this.selectedSeatIds.length;
  }

  confirmBooking(): void {
    if (this.selectedSeatIds.length === 0) {
      this.toastService.showWarning('Please select at least one seat');
      return;
    }

    const currentUser = this.authService.getUser();
    if (!this.authService.isLoggedIn() || !this.authService.getUser()?.id) {
      this.toastService.showInfo('Please login to book tickets');
      this.router.navigate(['/login'], { queryParams: { returnUrl: `/booking/${this.tripId}` } });
      return;
    }

    const bookingDto: CreateBookingDto = {
      userId: currentUser?.id ?? '',
      tripId: this.tripId!,
      seatIds: this.selectedSeatIds
    };

    this.bookingService.createBooking(bookingDto).subscribe({
      next: (response) => {
        this.stopTimer();
        this.toastService.showSuccess('Booking successful!');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.toastService.showError(err.error?.message || 'Booking failed');
      }
    });
  }
}
