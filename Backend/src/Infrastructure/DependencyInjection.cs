using System.Text;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings.GetValue<string>("Secret");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                    ValidAudience = jwtSettings.GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!))
                };
            });

            services.AddScoped<Application.Interfaces.IAuthService, Infrastructure.Services.AuthService>();
            
            services.AddScoped<Application.Interfaces.IBusRepository, Infrastructure.Persistence.Repositories.BusRepository>();
            services.AddScoped<Application.Interfaces.IBusService, Application.Services.BusService>();

            // Trip Management
            services.AddScoped<Domain.Interfaces.ITripRepository, Infrastructure.Repositories.TripRepository>();
            services.AddScoped<Application.Interfaces.ITripService, Infrastructure.Services.TripService>();

            // Route Management
            services.AddScoped<Domain.Interfaces.IRouteRepository, Infrastructure.Repositories.RouteRepository>();
            services.AddScoped<Application.Interfaces.IRouteService, Infrastructure.Services.RouteService>();
            services.AddScoped<Application.Interfaces.IPaymentService, Application.Services.PaymentService>();

            // Seat Management
            services.AddScoped<Application.Interfaces.ISeatService, Infrastructure.Services.SeatService>();
            services.AddScoped<Domain.Interfaces.IBookingRepository, Infrastructure.Persistence.Repositories.BookingRepository>();
            services.AddScoped<Application.Interfaces.IBookingService, Infrastructure.Services.BookingService>();

            // Extra Services
            services.AddScoped<Application.Interfaces.IBusTypeService, Application.Services.BusTypeService>();

            // Background Services
            services.AddHostedService<Infrastructure.Services.SeatLockBackgroundService>();

            // CMS Management
            services.AddScoped<Application.Interfaces.ICmsService, Application.Services.CmsService>();

            return services;
        }
    }
}
