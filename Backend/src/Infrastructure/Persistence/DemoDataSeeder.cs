using System;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Trip;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Bổ sung route, chuyến, user khách demo khi DB mới (Docker / dev). Idempotent.
    /// </summary>
    public static class DemoDataSeeder
    {
        public static readonly Guid DemoCustomerId = Guid.Parse("de000001-0000-4000-8000-000000000001");
        public static readonly Guid DemoRouteHcmDlId = Guid.Parse("de000002-0000-4000-8000-000000000001");
        public static readonly Guid DemoRouteHnHpId = Guid.Parse("de000003-0000-4000-8000-000000000001");

        private static readonly Guid SeededBusSleeper = Guid.Parse("55555555-5555-5555-5555-555555555555");
        private static readonly Guid SeededBusLimousine = Guid.Parse("66666666-6666-6666-6666-666666666666");

        public static async Task SeedAsync(IServiceProvider services, ILogger logger)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<ApplicationDbContext>();
            var tripService = sp.GetRequiredService<ITripService>();

            await EnsureDemoCustomerAsync(ctx, logger);
            await EnsureDemoRoutesAsync(ctx, logger);

            if (!await ctx.Trips.AnyAsync(t => t.RouteId == DemoRouteHcmDlId))
            {
                var dep = DateTime.UtcNow.Date.AddDays(1).AddHours(6);
                await tripService.CreateTripAsync(new CreateTripDto
                {
                    RouteId = DemoRouteHcmDlId,
                    BusId = SeededBusSleeper,
                    DepartureTime = dep,
                    ArrivalTime = dep.AddHours(8),
                    Price = 290_000m
                });
                logger.LogInformation("Demo: đã tạo chuyến TP.HCM → Đà Lạt (xe giường nằm).");
            }

            if (!await ctx.Trips.AnyAsync(t => t.RouteId == DemoRouteHnHpId))
            {
                var bus = await ctx.Buses.FindAsync(SeededBusLimousine);
                if (bus != null && bus.Status != BusStatus.Available)
                {
                    bus.Status = BusStatus.Available;
                    await ctx.SaveChangesAsync();
                }

                var dep = DateTime.UtcNow.Date.AddDays(1).AddHours(14);
                await tripService.CreateTripAsync(new CreateTripDto
                {
                    RouteId = DemoRouteHnHpId,
                    BusId = SeededBusLimousine,
                    DepartureTime = dep,
                    ArrivalTime = dep.AddHours(3),
                    Price = 180_000m
                });
                logger.LogInformation("Demo: đã tạo chuyến Hà Nội → Hải Phòng (limousine).");
            }

            var trips = await ctx.Trips.AsNoTracking().OrderBy(t => t.DepartureTime).Select(t => new { t.Id, t.RouteId }).ToListAsync();
            foreach (var t in trips)
            {
                var r = await ctx.Routes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == t.RouteId);
                logger.LogInformation("Demo trip: {TripId} — {Route}", t.Id, r == null ? "?" : $"{r.Origin} → {r.Destination}");
            }

            logger.LogInformation("Đăng nhập demo khách: user customer / email customer@demo.local / mật khẩu Customer@123 — UserId: {UserId}", DemoCustomerId);
        }

        private static async Task EnsureDemoCustomerAsync(ApplicationDbContext ctx, ILogger logger)
        {
            if (await ctx.Users.AnyAsync(u => u.Id == DemoCustomerId || u.Email == "customer@demo.local"))
                return;

            ctx.Users.Add(new User
            {
                Id = DemoCustomerId,
                UserName = "customer",
                Email = "customer@demo.local",
                FullName = "Khách demo",
                PhoneNumber = "0900111222",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                Role = RoleConstants.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            await ctx.SaveChangesAsync();
            logger.LogInformation("Demo: đã tạo user khách customer@demo.local.");
        }

        private static async Task EnsureDemoRoutesAsync(ApplicationDbContext ctx, ILogger logger)
        {
            if (!await ctx.Routes.AnyAsync(r => r.Id == DemoRouteHcmDlId))
            {
                ctx.Routes.Add(new Route
                {
                    Id = DemoRouteHcmDlId,
                    Origin = "TP.HCM",
                    Destination = "Đà Lạt",
                    Points = "QL20",
                    DistanceKm = 310,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await ctx.Routes.AnyAsync(r => r.Id == DemoRouteHnHpId))
            {
                ctx.Routes.Add(new Route
                {
                    Id = DemoRouteHnHpId,
                    Origin = "Hà Nội",
                    Destination = "Hải Phòng",
                    Points = "QL5",
                    DistanceKm = 120,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (ctx.ChangeTracker.HasChanges())
            {
                await ctx.SaveChangesAsync();
                logger.LogInformation("Demo: đã tạo tuyến demo.");
            }
        }
    }
}
