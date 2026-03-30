using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Application.DTOs;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusesController : ControllerBase
    {
        private readonly IBusService _busService;

        public BusesController(IBusService busService)
        {
            _busService = busService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var buses = await _busService.GetAllBusesAsync();
            return Ok(buses);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var bus = await _busService.GetBusByIdAsync(id);
            if (bus == null) return NotFound($"Bus with ID {id} not found.");
            return Ok(bus);
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBusDto createBusDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdBus = await _busService.CreateBusAsync(createBusDto);
                return CreatedAtAction(nameof(GetById), new { id = createdBus.Id }, createdBus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusDto updateBusDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var success = await _busService.UpdateBusAsync(id, updateBusDto);
                if (!success) return NotFound($"Bus with ID {id} not found or mismatch.");
                return NoContent();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id:guid}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _busService.DeleteBusAsync(id);
                if (!success) return NotFound($"Bus with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                // Most common error here is FK violation
                return BadRequest($"Không thể xóa xe này vì nó đang được gán cho các chuyến đi. (Error: {ex.Message})");
            }
        }
    }
}
