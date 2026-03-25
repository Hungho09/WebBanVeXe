using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.DTOs.Booking;
using Application.DTOs.Trip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpPost("lock-seat/{seatId}")]
        public async Task<IActionResult> LockSeat(Guid seatId, [FromBody] LockSeatRequestDto? request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest(new { message = "Cần UserId để giữ ghế." });
            var result = await _bookingService.LockSeatAsync(seatId, request.UserId);
            if (!result)
                return BadRequest(new { message = "Ghế không khả dụng hoặc đã có người giữ." });
            
            return Ok(new { message = "Giữ ghế thành công." });
        }

        [HttpPost("unlock-seat/{seatId}")]
        public async Task<IActionResult> UnlockSeat(Guid seatId, [FromBody] LockSeatRequestDto? request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest(new { message = "Cần UserId để mở khóa ghế." });
            var result = await _bookingService.UnlockSeatAsync(seatId, request.UserId);
            if (!result)
                return BadRequest(new { message = "Không thể mở khóa ghế." });
            
            return Ok(new { message = "Mở khóa ghế thành công." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto request)
        {
            if (request == null || request.SeatIds == null || request.SeatIds.Count == 0)
                return BadRequest(new { message = "Dữ liệu đặt vé không hợp lệ." });

            try
            {
                var response = await _bookingService.CreateBookingAsync(request);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (InvalidOperationException ex)
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
            var history = await _bookingService.GetUserBookingHistoryAsync(userId);
            return Ok(history);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            try
            {
                var ok = await _bookingService.CancelBookingAsync(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/approve-cancel")]
        public async Task<IActionResult> ApproveCancel(Guid id)
        {
            var ok = await _bookingService.ApproveCancelBookingAsync(id);
            if (!ok) return BadRequest(new { message = "Không thể duyệt hủy vé này (vé không ở trạng thái yêu cầu hủy hoặc không tồn tại)." });
            return Ok(new { message = "Đã duyệt hủy vé và giải phóng ghế." });
        }
    }
}
