using Application.DTOs.Booking;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            try
            {
                var result = await _bookingService.CreateBookingAsync(createBookingDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBookings(Guid userId)
        {
            var bookings = await _bookingService.GetUserBookingHistoryAsync(userId);
            return Ok(bookings);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Booking cancelled successfully" });
        }

        [HttpPost("{id}/request-cancel")]
        public async Task<IActionResult> RequestCancel(Guid id)
        {
            try
            {
                var result = await _bookingService.RequestCancelAsync(id);
                if (!result) return NotFound();
                return Ok(new { message = "Cancellation request sent successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/approve-cancel")]
        public async Task<IActionResult> ApproveCancel(Guid id)
        {
            var result = await _bookingService.ApproveCancelAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Cancellation approved and seats released" });
        }

        [HttpGet("pending-cancellations")]
        public async Task<IActionResult> GetPendingCancellations()
        {
            var result = await _bookingService.GetPendingCancellationsAsync();
            return Ok(result);
        }
    }
}
