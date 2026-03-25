using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Interfaces;
using Application.DTOs.Booking;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public BookingService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> LockSeatAsync(Guid seatId, Guid userId)
        {
            var seat = await _context.Seats.FindAsync(seatId);
            if (seat == null || seat.Status == SeatStatus.Booked)
                return false;

            // If already locked by someone else and not expired
            if (seat.Status == SeatStatus.Locked && seat.LockExpirationTime > DateTime.UtcNow && seat.LockedByUserId != userId)
                return false;

            seat.Status = SeatStatus.Locked;
            seat.LockedByUserId = userId;
            seat.LockExpirationTime = DateTime.UtcNow.AddMinutes(10);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlockSeatAsync(Guid seatId, Guid userId)
        {
            var seat = await _context.Seats.FindAsync(seatId);
            // Only unlock if it's locked and either owned by this user or just locked in general
            if (seat == null || seat.Status != SeatStatus.Locked || (seat.LockedByUserId != null && seat.LockedByUserId != userId))
                return false;

            seat.Status = SeatStatus.Available;
            seat.LockedByUserId = null;
            seat.LockExpirationTime = null;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) throw new Exception("Không tìm thấy thông tin người dùng.");

            var trip = await _context.Trips.Include(t => t.Route).FirstOrDefaultAsync(t => t.Id == dto.TripId);
            if (trip == null) throw new Exception("Chuyến đi không tồn tại.");

            var seats = await _context.Seats.Where(s => dto.SeatIds.Contains(s.Id)).ToListAsync();
            if (seats.Count != dto.SeatIds.Count) throw new Exception("Không tìm thấy một số ghế đã chọn.");

            foreach (var seat in seats)
            {
                if (seat.Status == SeatStatus.Booked)
                    throw new Exception($"Ghế {seat.SeatNumber} đã được người khác đặt.");
                
                // If it's locked by someone else and not expired, prevent booking
                if (seat.Status == SeatStatus.Locked && seat.LockExpirationTime > DateTime.UtcNow && seat.LockedByUserId != dto.UserId)
                    throw new Exception($"Ghế {seat.SeatNumber} hiện đang được giữ bởi người khác.");
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                TripId = dto.TripId,
                TotalAmount = seats.Count * trip.Price,
                BookingStatus = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);

            foreach (var seat in seats)
            {
                seat.Status = SeatStatus.Booked;
                var detail = new BookingDetail
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    SeatId = seat.Id,
                    Price = trip.Price
                };
                _context.BookingDetails.Add(detail);
            }

            await _context.SaveChangesAsync();

            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                UserName = user.UserName,
                TripId = booking.TripId,
                TotalAmount = booking.TotalAmount,
                BookingStatus = booking.BookingStatus.ToString(),
                CreatedAt = booking.CreatedAt,
                Details = seats.Select(s => new BookingDetailDto { 
                    Id = Guid.NewGuid(), 
                    SeatId = s.Id, 
                    SeatNumber = s.SeatNumber, 
                    Price = trip.Price 
                }).ToList()
            };
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(Guid id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return null;

            return MapToResponseDto(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUserBookingHistoryAsync(Guid userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return bookings.Select(MapToResponseDto);
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return false;
            if (booking.Trip == null) return false;

            // Story 3.3 Rule: Cannot cancel after departure time
            if (booking.Trip.DepartureTime <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Không thể hủy vé sau giờ khởi hành.");
            }

            // Simple cancel request logic (Epic 3.3 says CancelRequested status exists)
            booking.BookingStatus = BookingStatus.CancelRequested;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveCancelBookingAsync(Guid bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.BookingStatus != BookingStatus.CancelRequested) return false;

            booking.BookingStatus = BookingStatus.Cancelled;
            foreach (var detail in booking.BookingDetails)
            {
                if (detail.Seat != null)
                {
                    detail.Seat.Status = SeatStatus.Available;
                    detail.Seat.LockedByUserId = null;
                    detail.Seat.LockExpirationTime = null;
                }
            }

            await _context.SaveChangesAsync();

            // Notify customer
            try {
                await _notificationService.SendCancellationApprovalAsync(bookingId);
            } catch (Exception) { }

            return true;
        }

        private BookingResponseDto MapToResponseDto(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                UserName = booking.User?.UserName ?? "N/A",
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
