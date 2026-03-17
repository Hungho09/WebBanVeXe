using Application.DTOs.Trip;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> GetById(Guid id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null) return NotFound();
            return Ok(trip);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAll()
        {
            var trips = await _tripService.GetAllTripsAsync();
            return Ok(trips);
        }

        [HttpPost]
        public async Task<ActionResult<TripDto>> Create([FromBody] CreateTripDto dto)
        {
            var createdTrip = await _tripService.CreateTripAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdTrip.Id }, createdTrip);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTripDto dto)
        {
            var success = await _tripService.UpdateTripAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _tripService.DeleteTripAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
