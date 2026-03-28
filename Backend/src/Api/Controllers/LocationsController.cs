using System;
using System.Threading.Tasks;
using Application.DTOs.Location;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? searchTerm)
        {
            var locations = await _locationService.GetAllLocationsByProvinceAsync(searchTerm);
            return Ok(locations);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            if (location == null) return NotFound("Location not found");
            return Ok(location);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _locationService.CreateLocationAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _locationService.UpdateLocationAsync(id, updateDto);
            if (!success) return NotFound("Location not found");
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _locationService.DeleteLocationAsync(id);
            if (!success) return NotFound("Location not found");
            return NoContent();
        }

    }
}
