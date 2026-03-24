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

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LockSeatAsync(Guid seatId)
        {
            var seat = await _context.Seats.FindAsync(seatId);
            if (seat == null || seat.Status != SeatStatus.Available)
                return false;

            seat.Status = SeatStatus.Locked;
            // Optionally, we could set a lock expiration time here
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlockSeatAsync(Guid seatId)
        {
            var seat = await _context.Seats.FindAsync(seatId);
            if (seat == null || seat.Status != SeatStatus.Locked)
                return false;

            seat.Status = SeatStatus.Available;
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

            if (seats.Any(s => s.Status == SeatStatus.Booked))
                throw new Exception("Một hoặc nhiều ghế đã được người khác đặt.");

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
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return false;

            booking.BookingStatus = BookingStatus.Cancelled;
            foreach (var detail in booking.BookingDetails)
            {
                detail.Seat.Status = SeatStatus.Available;
            }

            await _context.SaveChangesAsync();
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
