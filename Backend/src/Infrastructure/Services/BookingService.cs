using Application.DTOs.Booking;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Persistence;

namespace Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ApplicationDbContext _context; // Using context directly for seat status checks for speed/simplicity in this pattern

        public BookingService(IBookingRepository bookingRepository, ApplicationDbContext context)
        {
            _bookingRepository = bookingRepository;
            _context = context;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
        {
            // 1. Validate Seats Availability
            var seats = await _context.Seats
                .Where(s => dto.SeatIds.Contains(s.Id) && s.TripId == dto.TripId)
                .ToListAsync();

            if (seats.Count != dto.SeatIds.Count)
                throw new Exception("Some seats were not found for this trip.");

            if (seats.Any(s => s.Status == SeatStatus.Booked || (s.Status == SeatStatus.Locked && s.LockExpirationTime > DateTime.UtcNow)))
                throw new Exception("One or more selected seats are no longer available.");

            // 2. Get Trip Price
            var trip = await _context.Trips.FindAsync(dto.TripId);
            if (trip == null) throw new Exception("Trip not found.");

            // 3. Create Booking
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                TripId = dto.TripId,
                TotalAmount = trip.Price * seats.Count,
                BookingStatus = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // 4. Create Booking Details and Update Seat Status
            foreach (var seat in seats)
            {
                booking.BookingDetails.Add(new BookingDetail
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    SeatId = seat.Id,
                    Price = trip.Price
                });

                seat.Status = SeatStatus.Booked;
            }

            // 5. Save to Database
            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return MapToResponse(booking);
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking == null ? null : MapToResponse(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUserBookingHistoryAsync(Guid userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return bookings.Select(MapToResponse);
        }

        public async Task<bool> RequestCancelAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            // Only Pending or Paid bookings can request cancellation
            if (booking.BookingStatus != BookingStatus.Pending && booking.BookingStatus != BookingStatus.Paid)
                return false;

            // Cannot cancel after departure time
            if (booking.Trip != null && booking.Trip.DepartureTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot request cancellation after the trip has departed.");

            booking.BookingStatus = BookingStatus.CancelRequested;
            await _bookingRepository.UpdateAsync(booking);
            return await _bookingRepository.SaveChangesAsync();
        }

        public async Task<bool> ApproveCancelAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.BookingStatus != BookingStatus.CancelRequested)
                return false;

            booking.BookingStatus = BookingStatus.Cancelled;

            // Release seats
            var seatIds = booking.BookingDetails.Select(bd => bd.SeatId).ToList();
            var seats = await _context.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync();
            foreach (var seat in seats)
            {
                seat.Status = SeatStatus.Available;
            }

            await _bookingRepository.UpdateAsync(booking);
            return await _bookingRepository.SaveChangesAsync();
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            if (booking.BookingStatus == BookingStatus.Cancelled) return true;

            booking.BookingStatus = BookingStatus.Cancelled;
            
            // Release seats
            var seatIds = booking.BookingDetails.Select(bd => bd.SeatId).ToList();
            var seats = await _context.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync();
            foreach (var seat in seats)
            {
                seat.Status = SeatStatus.Available;
            }

            await _bookingRepository.UpdateAsync(booking);
            return await _bookingRepository.SaveChangesAsync();
        }

        private BookingResponseDto MapToResponse(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                UserName = booking.User?.UserName ?? "Unknown",
                TripId = booking.TripId,
                TotalAmount = booking.TotalAmount,
                BookingStatus = booking.BookingStatus.ToString(),
                CreatedAt = booking.CreatedAt,
                Details = booking.BookingDetails.Select(bd => new BookingDetailDto
                {
                    Id = bd.Id,
                    SeatId = bd.SeatId,
                    SeatNumber = bd.Seat?.SeatNumber ?? "N/A",
                    Price = bd.Price
                }).ToList()
            };
        }
    }
}
