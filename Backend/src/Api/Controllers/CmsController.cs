using Application.DTOs.Cms;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CmsController : ControllerBase
    {
        private readonly ICmsService _cmsService;

        public CmsController(ICmsService cmsService)
        {
            _cmsService = cmsService;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var config = await _cmsService.GetConfigAsync(key);
            if (config == null) return NotFound(new { message = "Config not found" });
            return Ok(config);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] CmsConfigDto dto)
        {
            var success = await _cmsService.UpdateConfigAsync(dto);
            if (!success) return BadRequest(new { message = "Failed to update CMS config" });
            return Ok(new { message = "CMS config updated successfully" });
        }
    }
}
