using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAll()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteDto>> GetById(Guid id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null) return NotFound();
            return Ok(route);
        }

        [HttpPost]
        public async Task<ActionResult<RouteDto>> Create([FromBody] CreateRouteDto dto)
        {
            var createdRoute = await _routeService.CreateRouteAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdRoute.Id }, createdRoute);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRouteDto dto)
        {
            var success = await _routeService.UpdateRouteAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _routeService.DeleteRouteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
