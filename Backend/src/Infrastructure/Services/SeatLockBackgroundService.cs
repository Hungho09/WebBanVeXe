using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SeatLockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeatLockBackgroundService> _logger;

        public SeatLockBackgroundService(IServiceProvider serviceProvider, ILogger<SeatLockBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Seat Lock Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Seat Lock Background Service is stopping.");
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var expiredLocks = await context.Seats
                    .Where(s => s.Status == SeatStatus.Locked && s.LockExpirationTime < DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                if (expiredLocks.Any())
                {
                    _logger.LogInformation("Releasing {Count} expired seat locks.", expiredLocks.Count);
                    foreach (var seat in expiredLocks)
                    {
                        seat.Status = SeatStatus.Available;
                        seat.LockExpirationTime = null;
                        seat.LockedByUserId = null;
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
            }
        }
    }
}
