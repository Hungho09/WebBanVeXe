using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.SeedData
{
    public static class DemoDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Basic Check
            if (await context.Users.AnyAsync()) 
            {
                logger.LogInformation("Database already has data. Skipping seed.");
                return;
            }

            logger.LogInformation("Seeding Demo Data...");
            // (Remaining seeding logic can go here if needed)
            await Task.CompletedTask;
        }
    }
}
