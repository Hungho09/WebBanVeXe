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

        public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            var paidBookings = await _context.Bookings
                .Where(b => b.BookingStatus == BookingStatus.Paid && b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                .ToListAsync();

            var totalRevenue = paidBookings.Sum(b => b.TotalAmount);
            
            var dailyRevenue = paidBookings
                .GroupBy(b => b.CreatedAt.Date)
                .Select(g => new RevenueByDayDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(b => b.TotalAmount),
                    BookingCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            return new RevenueReportDto
            {
                TotalRevenue = totalRevenue,
                TotalPaidBookings = paidBookings.Count,
                DailyRevenue = dailyRevenue
            };
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalBookings = await _context.Bookings.CountAsync();
            var totalRevenue = await _context.Bookings
                .Where(b => b.BookingStatus == BookingStatus.Paid)
                .SumAsync(b => b.TotalAmount);
            
            var totalUsers = await _context.Users.CountAsync();
            var activeTrips = await _context.Trips.CountAsync(t => t.Status == TripStatus.Active);
            
            // Story 4.3: Most popular route calculation
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

            return new DashboardStatsDto
            {
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                TotalUsers = totalUsers,
                ActiveTrips = activeTrips,
                MostPopularRoute = routeName
            };
        }
    }
}
