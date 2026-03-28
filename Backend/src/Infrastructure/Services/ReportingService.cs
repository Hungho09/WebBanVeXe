using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Reporting;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Infrastructure.Services
{
    public class ReportingService : IReportingService
    {
        private readonly ApplicationDbContext _context;

        public ReportingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(RevenueQueryDto query)
        {
            DateTime startDate;
            DateTime endDate = query.EndDate ?? DateTime.UtcNow;

            if (query.StartDate.HasValue)
            {
                startDate = query.StartDate.Value;
            }
            else
            {
                // Default ranges based on mode
                startDate = query.Mode switch
                {
                    ReportingMode.Month => new DateTime(endDate.Year, 1, 1), // Current year's months
                    ReportingMode.Year => endDate.AddYears(-5), // Last 5 years
                    _ => endDate.AddMonths(-1) // Last 30 days
                };
            }

            // Ensure we include the full start day
            startDate = startDate.Date;

            var paidBookings = await _context.Bookings
                .Where(b => b.BookingStatus == BookingStatus.Paid && b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                .ToListAsync();

            var totalRevenue = paidBookings.Sum(b => b.TotalAmount);
            
            var revenueDetails = paidBookings
                .GroupBy(b => GetGroupKey(b.CreatedAt, query.Mode))
                .Select(g => new RevenueDataPointDto
                {
                    Date = g.Key,
                    Label = GetLabel(g.Key, query.Mode),
                    Revenue = g.Sum(b => b.TotalAmount),
                    BookingCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            return new RevenueReportDto
            {
                TotalRevenue = totalRevenue,
                TotalPaidBookings = paidBookings.Count,
                RevenueDetails = revenueDetails
            };
        }

        private DateTime GetGroupKey(DateTime date, ReportingMode mode)
        {
            return mode switch
            {
                ReportingMode.Month => new DateTime(date.Year, date.Month, 1),
                ReportingMode.Year => new DateTime(date.Year, 1, 1),
                _ => date.Date
            };
        }

        private string GetLabel(DateTime date, ReportingMode mode)
        {
            return mode switch
            {
                ReportingMode.Month => date.ToString("yyyy-MM"),
                ReportingMode.Year => date.ToString("yyyy"),
                _ => date.ToString("yyyy-MM-dd")
            };
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;
            // Calculate start of current week (Monday)
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = now.AddDays(-1 * diff).Date;

            var allPaidBookings = _context.Bookings.Where(b => b.BookingStatus == BookingStatus.Paid);
            
            // Statistics for THIS WEEK only as per UI title
            var thisWeekBookings = allPaidBookings.Where(b => b.CreatedAt >= startOfWeek);
            
            var totalBookings = await thisWeekBookings.CountAsync();
            var totalRevenue = await thisWeekBookings.SumAsync(b => b.TotalAmount);
            
            var totalUsers = await _context.Users.CountAsync();
            var activeTrips = await _context.Trips.CountAsync(t => t.Status == TripStatus.Active);
            
            // Story 4.3: Most popular route calculation (All time)
            var popularRoute = await _context.Bookings
                .GroupBy(b => b.Trip!.RouteId)
                .Select(g => new { RouteId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            string routeName = "N/A";
            if (popularRoute != null)
            {
                var route = await _context.Routes.FindAsync(popularRoute.RouteId);
                if (route != null)
                {
                    routeName = $"{route.Origin} - {route.Destination}";
                }
            }

            // Story 4.4: Recent bookings for Dashboard
            var recentBookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Trip)
                    .ThenInclude(t => t!.Route)
                .Include(b => b.Trip)
                    .ThenInclude(t => t!.Bus)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(b => new RecentBookingDto
                {
                    Id = b.Id,
                    CustomerName = b.User != null ? b.User.FullName : "Khách vãng lai",
                    CustomerAvatar = "https://i.pravatar.cc/150?u=" + b.UserId,
                    RouteName = (b.Trip != null && b.Trip.Route != null)
                        ? b.Trip.Route.Origin + " - " + b.Trip.Route.Destination
                        : "Unknown",
                    DepartureTime = b.Trip != null ? b.Trip.DepartureTime : DateTime.MinValue,
                    BusPlate = (b.Trip != null && b.Trip.Bus != null) ? b.Trip.Bus.PlateNumber : "N/A",
                    Status = b.BookingStatus.ToString()
                })
                .ToListAsync();

            return new DashboardStatsDto
            {
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                TotalUsers = totalUsers,
                ActiveTrips = activeTrips,
                MostPopularRoute = routeName,
                RecentBookings = recentBookings
            };
        }

        public async Task<OccupancyReportDto> GetOccupancyReportAsync(OccupancyQueryDto query)
        {
            var endDate = query.EndDate ?? DateTime.UtcNow;
            var startDate = query.StartDate ?? endDate.AddMonths(-1);

            var tripsQuery = _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Route)
                .Include(t => t.Seats)
                .Where(t => t.DepartureTime >= startDate && t.DepartureTime <= endDate);

            if (query.RouteId.HasValue)
            {
                tripsQuery = tripsQuery.Where(t => t.RouteId == query.RouteId);
            }

            var trips = await tripsQuery
                .OrderByDescending(t => t.DepartureTime)
                .ToListAsync();

            var tripOccupancies = trips.Select(t => {
                // BVX-138: Total seats from bus
                // BVX-139: Count booked seats
                int totalSeats = t.Bus?.SeatCount ?? 0;
                int bookedSeats = t.Seats.Count(s => s.Status == SeatStatus.Booked);
                
                // BVX-140: Calculate occupancy percentage
                double percentage = totalSeats > 0 ? (double)bookedSeats / totalSeats * 100 : 0;

                return new TripOccupancyDto
                {
                    TripId = t.Id,
                    RouteName = $"{t.Route?.Origin} - {t.Route?.Destination}",
                    DepartureTime = t.DepartureTime,
                    BusPlate = t.Bus?.PlateNumber ?? "N/A",
                    TotalSeats = totalSeats,
                    BookedSeats = bookedSeats,
                    OccupancyPercentage = Math.Round(percentage, 2)
                };
            }).ToList();

            return new OccupancyReportDto
            {
                Trips = tripOccupancies,
                TotalTrips = tripOccupancies.Count,
                AverageOccupancy = tripOccupancies.Any() ? Math.Round(tripOccupancies.Average(t => t.OccupancyPercentage), 2) : 0
            };
        }
    }
}
