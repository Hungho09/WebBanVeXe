using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.DTOs.Reporting
{
    public class RevenueQueryDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReportingMode Mode { get; set; } = ReportingMode.Day;
    }

    public class RevenueReportDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalPaidBookings { get; set; }
        public List<RevenueDataPointDto> RevenueDetails { get; set; } = new();
    }

    public class RevenueDataPointDto
    {
        public DateTime Date { get; set; }
        public string Label { get; set; } = ""; // E.g., "2024-03", "2024"
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
        public List<RecentBookingDto> RecentBookings { get; set; } = new();
    }

    public class RecentBookingDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerAvatar { get; set; } = "";
        public string RouteName { get; set; } = "";
        public DateTime DepartureTime { get; set; }
        public string BusPlate { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
