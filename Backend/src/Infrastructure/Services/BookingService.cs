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
        private readonly IBookingRepository _bookingRepository;

        public BookingService(ApplicationDbContext context, IBookingRepository bookingRepository)
        {
            _context = context;
            _bookingRepository = bookingRepository;
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
            if (dto.SeatIds.Count != dto.SeatIds.Distinct().Count())
                throw new InvalidOperationException("Không được chọn trùng ghế trong cùng một lần đặt.");

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) throw new InvalidOperationException("Không tìm thấy thông tin người dùng.");

            var trip = await _context.Trips.Include(t => t.Route).FirstOrDefaultAsync(t => t.Id == dto.TripId);
            if (trip == null) throw new InvalidOperationException("Chuyến đi không tồn tại.");

            var seats = await _context.Seats
                .Where(s => dto.SeatIds.Contains(s.Id) && s.TripId == dto.TripId)
                .ToListAsync();
            if (seats.Count != dto.SeatIds.Count)
                throw new InvalidOperationException("Không tìm thấy ghế hoặc ghế không thuộc chuyến đã chọn.");

            ValidateSeatsForBooking(seats);

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                TripId = dto.TripId,
                TotalAmount = seats.Count * trip.Price,
                BookingStatus = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var seat in seats)
            {
                seat.Status = SeatStatus.Booked;
                booking.BookingDetails.Add(new BookingDetail
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    SeatId = seat.Id,
                    Price = trip.Price
                });
            }

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                UserName = user.UserName,
                TripId = booking.TripId,
                TotalAmount = booking.TotalAmount,
                BookingStatus = booking.BookingStatus.ToString(),
                CreatedAt = booking.CreatedAt,
                Details = booking.BookingDetails.Select(bd =>
                {
                    var seat = seats.First(s => s.Id == bd.SeatId);
                    return new BookingDetailDto
                    {
                        Id = bd.Id,
                        SeatId = bd.SeatId,
                        SeatNumber = seat.SeatNumber,
                        Price = bd.Price
                    };
                }).ToList()
            };
        }

        /// <summary>
        /// Ghế phải thuộc chuyến (đã lọc trước đó). Cho phép Available hoặc Locked (đã giữ ghế trước khi thanh toán).
        /// </summary>
        private static void ValidateSeatsForBooking(IReadOnlyCollection<Seat> seats)
        {
            foreach (var s in seats)
            {
                switch (s.Status)
                {
                    case SeatStatus.Available:
                    case SeatStatus.Locked:
                        continue;
                    case SeatStatus.Booked:
                        throw new InvalidOperationException("Một hoặc nhiều ghế đã được đặt.");
                    case SeatStatus.Maintenance:
                        throw new InvalidOperationException("Một hoặc nhiều ghế đang bảo trì, không thể đặt.");
                    case SeatStatus.Reserved:
                        throw new InvalidOperationException("Một hoặc nhiều ghế đang được giữ chỗ (Reserved), không thể đặt.");
                    default:
                        throw new InvalidOperationException("Một hoặc nhiều ghế không ở trạng thái có thể đặt.");
                }
            }
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking == null ? null : MapToResponseDto(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUserBookingHistoryAsync(Guid userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return bookings.Select(MapToResponseDto);
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            booking.BookingStatus = BookingStatus.Cancelled;
            foreach (var detail in booking.BookingDetails)
            {
                var seat = detail.Seat ?? await _context.Seats.FindAsync(detail.SeatId);
                if (seat != null)
                    seat.Status = SeatStatus.Available;
            }

            await _bookingRepository.UpdateAsync(booking);
            return await _bookingRepository.SaveChangesAsync();
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
