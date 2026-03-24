using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusTypesController : ControllerBase
    {
        private readonly IBusTypeService _busTypeService;

        public BusTypesController(IBusTypeService busTypeService)
        {
            _busTypeService = busTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusTypeDto>>> GetAll()
        {
            var result = await _busTypeService.GetAllAsync();
            return Ok(result);
        }
    }
}
