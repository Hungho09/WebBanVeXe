using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Interfaces;
using Application.DTOs.Booking;
using Application.DTOs.Invoice;
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
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync();
                throw new InvalidOperationException("Ghế bạn chọn đã được người khác đặt hoặc hết thời gian chờ. Vui lòng làm mới lại trang.");
            }
            catch (Exception)
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
                .Include(b => b.Invoice)
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

        public async Task<IEnumerable<BookingResponseDto>> GetCancelRequestsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Seat)
                .Include(b => b.User)
                .Include(b => b.Trip!)
                    .ThenInclude(t => t.Route)
                .Where(b => b.BookingStatus == BookingStatus.CancelRequested)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return bookings.Select(MapToResponseDto);
        }

        public async Task<bool> RequestCancelAsync(Guid bookingId, Guid userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return false;
            if (booking.UserId != userId)
                throw new InvalidOperationException("Bạn không có quyền yêu cầu hủy vé này.");

            if (booking.BookingStatus == BookingStatus.CancelRequested)
                throw new InvalidOperationException("Vé này đã có yêu cầu hủy trước đó.");

            if (booking.BookingStatus == BookingStatus.Cancelled)
                throw new InvalidOperationException("Vé này đã được hủy.");

            booking.BookingStatus = BookingStatus.CancelRequested;
            await _bookingRepository.UpdateAsync(booking);
            return await _bookingRepository.SaveChangesAsync();
        }

        public async Task<bool> ApproveCancelAsync(Guid bookingId, Guid adminUserId)
        {
            var admin = await _context.Users.FindAsync(adminUserId);
            if (admin == null || !string.Equals(admin.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chỉ quản trị viên mới được duyệt hủy.");

            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;
            if (booking.BookingStatus != BookingStatus.CancelRequested)
                throw new InvalidOperationException("Vé chưa ở trạng thái chờ duyệt hủy.");

            booking.BookingStatus = BookingStatus.Cancelled;
            await ReleaseSeatsForBookingAsync(booking);
            await _bookingRepository.UpdateAsync(booking);
            var ok = await _bookingRepository.SaveChangesAsync();
            if (ok)
            {
                try { await _notificationService.SendCancellationApprovalAsync(bookingId); } catch { }
            }
            return ok;
        }

        private async Task ReleaseSeatsForBookingAsync(Booking booking)
        {
            foreach (var detail in booking.BookingDetails)
            {
                var seat = detail.Seat ?? await _context.Seats.FindAsync(detail.SeatId);
                if (seat == null) continue;
                seat.Status = SeatStatus.Available;
                seat.LockExpirationTime = null;
                seat.LockedByUserId = null;
            }
        }

        private async Task<BookingResponseDto> MapToResponseDtoAsync(Booking booking, List<Seat> seats)
        {
            var pickupPoint = booking.PickupPointId.HasValue ? await _context.Locations.FindAsync(booking.PickupPointId) : null;
            var dropoffPoint = booking.DropoffPointId.HasValue ? await _context.Locations.FindAsync(booking.DropoffPointId) : null;

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
                RouteName = booking.Trip?.Route != null
                    ? $"{booking.Trip.Route.Origin} - {booking.Trip.Route.Destination}"
                    : "N/A",
                DepartureTime = booking.Trip?.DepartureTime,
                ArrivalTime = booking.Trip?.ArrivalTime,
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
                }).ToList(),
                Invoice = booking.Invoice == null ? null : new InvoiceDto
                {
                    Id = booking.Invoice.Id,
                    InvoiceNumber = booking.Invoice.InvoiceNumber,
                    BookingId = booking.Invoice.BookingId,
                    CustomerName = booking.Invoice.CustomerName,
                    CustomerEmail = booking.Invoice.CustomerEmail,
                    TotalAmount = booking.Invoice.TotalAmount,
                    CreatedAt = booking.Invoice.CreatedAt,
                    Status = booking.Invoice.Status.ToString()
                }
            };
        }
    }
}
