using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
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

        public async Task<Guid> CreateBookingAsync(string userName, Guid tripId, Guid[] seatIds)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) throw new Exception("Không tìm thấy thông tin người dùng.");

            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null) throw new Exception("Chuyến đi không tồn tại.");

            var seats = await _context.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync();
            if (seats.Count != seatIds.Length) throw new Exception("Không tìm thấy một số ghế đã chọn.");

            if (seats.Any(s => s.Status == SeatStatus.Booked))
                throw new Exception("Một hoặc nhiều ghế đã được người khác đặt.");

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TripId = tripId,
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
            return booking.Id;
        }
    }
}
