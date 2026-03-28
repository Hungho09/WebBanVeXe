using Application.Interfaces;
using Application.DTOs.Reporting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IReportingService _reportingService;

        public DashboardController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _reportingService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] RevenueQueryDto query)
        {
            var report = await _reportingService.GetRevenueReportAsync(query);
            return Ok(report);
        }

        [HttpGet("occupancy")]
        public async Task<IActionResult> GetOccupancy([FromQuery] OccupancyQueryDto query)
        {
            var report = await _reportingService.GetOccupancyReportAsync(query);
            return Ok(report);
        }
    }
}
