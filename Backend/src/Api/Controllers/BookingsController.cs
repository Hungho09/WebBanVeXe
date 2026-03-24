using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class CreateBookingDto
    {
        public string UserId { get; set; } = string.Empty;
        public Guid TripId { get; set; }
        public Guid[] SeatIds { get; set; } = Array.Empty<Guid>();
    }

    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("lock-seat/{seatId}")]
        public async Task<IActionResult> LockSeat(Guid seatId)
        {
            var result = await _bookingService.LockSeatAsync(seatId);
            if (!result)
                return BadRequest(new { message = "Ghế không khả dụng hoặc đã có người giữ." });
            
            return Ok(new { message = "Giữ ghế thành công." });
        }

        [HttpPost("unlock-seat/{seatId}")]
        public async Task<IActionResult> UnlockSeat(Guid seatId)
        {
            var result = await _bookingService.UnlockSeatAsync(seatId);
            if (!result)
                return BadRequest(new { message = "Không thể mở khóa ghế." });
            
            return Ok(new { message = "Mở khóa ghế thành công." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto request)
        {
            if (request == null || request.SeatIds == null || request.SeatIds.Length == 0)
                return BadRequest(new { message = "Dữ liệu đặt vé không hợp lệ." });

            try
            {
                var bookingId = await _bookingService.CreateBookingAsync(request.UserId, request.TripId, request.SeatIds);
                return Ok(new { bookingId, message = "Đặt vé thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
