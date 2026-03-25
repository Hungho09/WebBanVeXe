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
    }
}
