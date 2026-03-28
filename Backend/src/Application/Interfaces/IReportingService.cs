using System;
using System.Threading.Tasks;
using Application.DTOs.Reporting;

namespace Application.Interfaces
{
    public interface IReportingService
    {
        Task<RevenueReportDto> GetRevenueReportAsync(RevenueQueryDto query);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<OccupancyReportDto> GetOccupancyReportAsync(OccupancyQueryDto query);
    }
}
