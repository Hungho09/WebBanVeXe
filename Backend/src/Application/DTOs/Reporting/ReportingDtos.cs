using System;
using System.Collections.Generic;

namespace Application.DTOs.Reporting
{
    public class RevenueReportDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalPaidBookings { get; set; }
        public List<RevenueByDayDto> DailyRevenue { get; set; } = new();
    }

    public class RevenueByDayDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveTrips { get; set; }
        public string MostPopularRoute { get; set; } = "N/A";
    }
}
