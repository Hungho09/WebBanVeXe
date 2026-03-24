using Application.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBookingService
    {
        Task<bool> LockSeatAsync(Guid seatId);
        Task<bool> UnlockSeatAsync(Guid seatId);
        
        // Simplified DTO-based booking
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto);
        
        Task<BookingResponseDto?> GetBookingByIdAsync(Guid id);
        Task<IEnumerable<BookingResponseDto>> GetUserBookingHistoryAsync(Guid userId);
        Task<bool> CancelBookingAsync(Guid bookingId);
    }
}
