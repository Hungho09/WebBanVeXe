using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Booking;

namespace Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<BookingResponseDto?> GetBookingByIdAsync(Guid id);
        Task<IEnumerable<BookingResponseDto>> GetUserBookingHistoryAsync(Guid userId);
        Task<bool> RequestCancelAsync(Guid bookingId);
        Task<bool> ApproveCancelAsync(Guid bookingId);
        Task<bool> CancelBookingAsync(Guid bookingId); // Keeps direct cancel for admin/system use
    }
}
