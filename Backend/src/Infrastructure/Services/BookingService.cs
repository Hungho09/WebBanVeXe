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
        private readonly INotificationService _notificationService;
        private readonly IBookingRepository _bookingRepository;

        public BookingService(
            ApplicationDbContext context, 
            INotificationService notificationService,
            IBookingRepository bookingRepository)
        {
            _context = context;
            _notificationService = notificationService;
            _bookingRepository = bookingRepository;
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
            if (dto.SeatIds.Count != dto.SeatIds.Distinct().Count())
                throw new InvalidOperationException("Không được chọn trùng ghế trong cùng một lần đặt.");

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) throw new InvalidOperationException("Không tìm thấy thông tin người dùng.");

            var trip = await _context.Trips.Include(t => t.Route).FirstOrDefaultAsync(t => t.Id == dto.TripId);
            if (trip == null) throw new InvalidOperationException("Chuyến đi không tồn tại.");

            await using IDbContextTransaction tx = await _context.Database.BeginTransactionAsync();
            try
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

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return await MapToResponseDtoAsync(booking, seats);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

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
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                .Include(b => b.PickupPoint)
                .Include(b => b.DropoffPoint)
                .FirstOrDefaultAsync(b => b.Id == id);
            
            return booking == null ? null : MapToResponseDto(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                .Include(b => b.PickupPoint)
                .Include(b => b.DropoffPoint)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
                
            return bookings.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUserBookingHistoryAsync(Guid userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Seat)
                .Include(b => b.PickupPoint)
                .Include(b => b.DropoffPoint)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
                
            return bookings.Select(MapToResponseDto);
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return false;
            
            if (booking.Trip != null && booking.Trip.DepartureTime <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Không thể hủy vé sau giờ khởi hành.");
            }

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

            try {
                await _notificationService.SendCancellationApprovalAsync(bookingId);
            } catch (Exception) { }

            return true;
        }

        private async Task<BookingResponseDto> MapToResponseDtoAsync(Booking booking, List<Seat> seats)
        {
            var pickupPoint = booking.PickupPointId.HasValue ? await _context.StopPoints.FindAsync(booking.PickupPointId) : null;
            var dropoffPoint = booking.DropoffPointId.HasValue ? await _context.StopPoints.FindAsync(booking.DropoffPointId) : null;

            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                UserName = (await _context.Users.FindAsync(booking.UserId))?.UserName ?? "N/A",
                TripId = booking.TripId,
                TotalAmount = booking.TotalAmount,
                BookingStatus = booking.BookingStatus.ToString(),
                CreatedAt = booking.CreatedAt,
                PickupPointId = booking.PickupPointId,
                PickupPointName = pickupPoint?.Name,
                DropoffPointId = booking.DropoffPointId,
                DropoffPointName = dropoffPoint?.Name,
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
