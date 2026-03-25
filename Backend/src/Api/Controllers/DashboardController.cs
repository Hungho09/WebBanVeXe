using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IActionResult> GetRevenue(DateTime? start, DateTime? end)
        {
            var startDate = start ?? DateTime.UtcNow.AddMonths(-1);
            var endDate = end ?? DateTime.UtcNow;
            
            var report = await _reportingService.GetRevenueReportAsync(startDate, endDate);
            return Ok(report);
        }
    }
}
