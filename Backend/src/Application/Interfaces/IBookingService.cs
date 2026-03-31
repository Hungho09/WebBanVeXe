using Application.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBookingService
    {
        Task<bool> LockSeatAsync(Guid seatId, Guid userId);
        Task<bool> UnlockSeatAsync(Guid seatId, Guid userId);
        
        // Simplified DTO-based booking
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto);
        
        Task<BookingResponseDto?> GetBookingByIdAsync(Guid id);
        Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync();
        Task<IEnumerable<BookingResponseDto>> GetUserBookingHistoryAsync(Guid userId);
        Task<IEnumerable<BookingResponseDto>> GetCancelRequestsAsync();
        Task<bool> RequestCancelAsync(Guid bookingId, Guid userId);
        Task<bool> ApproveCancelAsync(Guid bookingId, Guid adminUserId);
        Task<bool> ConfirmPaymentAsync(Guid bookingId, Guid adminUserId);
    }
}
