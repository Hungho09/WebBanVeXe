using System;
using System.Threading.Tasks;
using Application.DTOs.Reporting;

namespace Application.Interfaces
{
    public interface IReportingService
    {
        Task<RevenueReportDto> GetRevenueReportAsync(DateTime startDate, DateTime endDate);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
