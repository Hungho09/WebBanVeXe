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
using Microsoft.EntityFrameworkCore.Storage;
using Infrastructure.Persistence;

namespace Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
<<<<<<< HEAD
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
=======
        private readonly IBookingRepository _bookingRepository;
        private readonly ITripService _tripService;

        public BookingService(
            ApplicationDbContext context,
            IBookingRepository bookingRepository,
            ITripService tripService)
        {
            _context = context;
            _bookingRepository = bookingRepository;
            _tripService = tripService;
        }

        public Task<bool> LockSeatAsync(Guid seatId, Guid userId) =>
            _tripService.LockSeatAsync(seatId, userId);

        public Task<bool> UnlockSeatAsync(Guid seatId, Guid userId) =>
            _tripService.UnlockSeatAsync(seatId, userId);
>>>>>>> 9197da9e81287ec8d327737d1f37f56927fc8b7e

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
        {
            if (dto.SeatIds.Count != dto.SeatIds.Distinct().Count())
                throw new InvalidOperationException("Không được chọn trùng ghế trong cùng một lần đặt.");

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) throw new InvalidOperationException("Không tìm thấy thông tin người dùng.");

            var trip = await _context.Trips.Include(t => t.Route).FirstOrDefaultAsync(t => t.Id == dto.TripId);
            if (trip == null) throw new InvalidOperationException("Chuyến đi không tồn tại.");

<<<<<<< HEAD
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
=======
            await using IDbContextTransaction tx = await _context.Database.BeginTransactionAsync();
            try
>>>>>>> 9197da9e81287ec8d327737d1f37f56927fc8b7e
            {
                var seats = await _context.Seats
                    .Where(s => dto.SeatIds.Contains(s.Id) && s.TripId == dto.TripId)
                    .ToListAsync();
                if (seats.Count != dto.SeatIds.Count)
                    throw new InvalidOperationException("Không tìm thấy ghế hoặc ghế không thuộc chuyến đã chọn.");

                ValidateSeatsForBooking(seats, dto.UserId);

                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    TripId = dto.TripId,
                    TotalAmount = seats.Count * trip.Price,
                    BookingStatus = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    PickupPointId = dto.PickupPointId,
                    DropoffPointId = dto.DropoffPointId
                };

                foreach (var seat in seats)
                {
                    seat.Status = SeatStatus.Booked;
                    seat.LockExpirationTime = null;
                    seat.LockedByUserId = null;
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
                await tx.CommitAsync();

                return new BookingResponseDto
                {
                    Id = booking.Id,
                    UserId = booking.UserId,
                    UserName = user.UserName,
                    TripId = booking.TripId,
                    TotalAmount = booking.TotalAmount,
                    BookingStatus = booking.BookingStatus.ToString(),
                    CreatedAt = booking.CreatedAt,
                    PickupPointId = booking.PickupPointId,
                    PickupPointName = booking.PickupPointId.HasValue ? (await _context.StopPoints.FindAsync(booking.PickupPointId))?.Name : null,
                    DropoffPointId = booking.DropoffPointId,
                    DropoffPointName = booking.DropoffPointId.HasValue ? (await _context.StopPoints.FindAsync(booking.DropoffPointId))?.Name : null,
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
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Ghế phải thuộc chuyến (đã lọc trước đó). Locked chỉ hợp lệ nếu cùng user đang giữ và chưa hết hạn.
        /// </summary>
        private static void ValidateSeatsForBooking(IReadOnlyCollection<Seat> seats, Guid bookingUserId)
        {
            var now = DateTime.UtcNow;
            foreach (var s in seats)
            {
                switch (s.Status)
                {
                    case SeatStatus.Available:
                        continue;
                    case SeatStatus.Locked:
                        if (!s.LockExpirationTime.HasValue || s.LockExpirationTime <= now)
                            throw new InvalidOperationException("Thời gian giữ ghế đã hết. Vui lòng chọn lại ghế.");
                        if (s.LockedByUserId != bookingUserId)
                            throw new InvalidOperationException("Một hoặc nhiều ghế đang được người khác giữ.");
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
<<<<<<< HEAD
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

=======
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
>>>>>>> 9197da9e81287ec8d327737d1f37f56927fc8b7e
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
<<<<<<< HEAD
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
=======
                var seat = detail.Seat ?? await _context.Seats.FindAsync(detail.SeatId);
                if (seat != null)
                {
                    seat.Status = SeatStatus.Available;
                    seat.LockExpirationTime = null;
                    seat.LockedByUserId = null;
                }
            }

            await _bookingRepository.UpdateAsync(booking);
            return await _bookingRepository.SaveChangesAsync();
>>>>>>> 9197da9e81287ec8d327737d1f37f56927fc8b7e
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
                PickupPointId = booking.PickupPointId,
                PickupPointName = booking.PickupPoint?.Name,
                DropoffPointId = booking.DropoffPointId,
                DropoffPointName = booking.DropoffPoint?.Name,
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
