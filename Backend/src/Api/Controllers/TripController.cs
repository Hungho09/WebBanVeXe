using Application.DTOs.Trip;
using Application.DTOs.Booking;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAll()
        {
            var trips = await _tripService.GetAllTripsAsync();
            return Ok(trips);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TripDto>>> Search([FromQuery] string? origin, [FromQuery] string? destination, [FromQuery] DateTime? date)
        {
            var trips = await _tripService.SearchTripsAsync(origin, destination, date);
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> GetById(Guid id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null) return NotFound();
            return Ok(trip);
        }

        [HttpPost]
        public async Task<ActionResult<TripDto>> Create([FromBody] CreateTripDto dto)
        {
            try
            {
                var createdTrip = await _tripService.CreateTripAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdTrip.Id }, createdTrip);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTripDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                Console.WriteLine($"Model Validation Failed: {errors}");
                return BadRequest(new { message = "Dữ liệu không hợp lệ: " + errors });
            }

            Console.WriteLine($"Updating Trip: id={id}, routeId={dto?.RouteId}, busId={dto?.BusId}, depTime={dto?.DepartureTime}, arrTime={dto?.ArrivalTime}, status={dto?.Status}");
            
            if (dto == null)
            {
                Console.WriteLine("Update DTO is null");
                return BadRequest(new { message = "Request body is null or invalid JSON." });
            }

            try
            {
                var success = await _tripService.UpdateTripAsync(id, dto);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _tripService.DeleteTripAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("{id}/seats")]
        public async Task<ActionResult<IEnumerable<SeatDto>>> GetSeats(Guid id)
        {
            var seats = await _tripService.GetSeatsByTripIdAsync(id);
            return Ok(seats);
        }

        [HttpPost("seats/{seatId}/lock")]
        public async Task<IActionResult> LockSeat(Guid seatId, [FromBody] LockSeatRequestDto? request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest(new { message = "Cần UserId để giữ ghế." });
            var success = await _tripService.LockSeatAsync(seatId, request.UserId);
            if (!success) return BadRequest(new { message = "Seat could not be locked. It might be already booked or locked by someone else." });
            return Ok(new { message = "Seat locked successfully" });
        }

        [HttpPost("seats/{seatId}/unlock")]
        public async Task<IActionResult> UnlockSeat(Guid seatId, [FromBody] LockSeatRequestDto? request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest(new { message = "Cần UserId để mở khóa ghế." });
            var success = await _tripService.UnlockSeatAsync(seatId, request.UserId);
            if (!success) return BadRequest(new { message = "Seat could not be unlocked." });
            return Ok(new { message = "Seat unlocked successfully" });
        }

        [HttpGet("{id}/points")]
        public async Task<ActionResult<IEnumerable<TripPointDto>>> GetPoints(Guid id)
        {
            var points = await _tripService.GetTripPointsAsync(id);
            return Ok(points);
        }
    }
}
