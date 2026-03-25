using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Application.DTOs.Route;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("locations")]
        public async Task<IActionResult> GetLocations()
        {
            var (origins, destinations) = await _routeService.GetDistinctLocationsAsync();
            return Ok(new { origins, destinations });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null) return NotFound($"Route with ID {id} not found.");
            return Ok(route);
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRouteDto createRouteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdRoute = await _routeService.CreateRouteAsync(createRouteDto);
                return CreatedAtAction(nameof(GetById), new { id = createdRoute.Id }, createdRoute);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRouteDto updateRouteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var success = await _routeService.UpdateRouteAsync(id, updateRouteDto);
                if (!success) return NotFound($"Route with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _routeService.DeleteRouteAsync(id);
            if (!success) return NotFound($"Route with ID {id} not found.");
            return NoContent();
        }
    }
}
