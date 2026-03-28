using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Route> Routes { get; set; }
        DbSet<Bus> Buses { get; set; }
        DbSet<Trip> Trips { get; set; }
        DbSet<Seat> Seats { get; set; }
        DbSet<Booking> Bookings { get; set; }
        DbSet<BookingDetail> BookingDetails { get; set; }
        DbSet<Payment> Payments { get; set; }
        DbSet<Invoice> Invoices { get; set; }
        DbSet<Notification> Notifications { get; set; }
        DbSet<BusType> BusTypes { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<Province> Provinces { get; set; }
        DbSet<RouteStop> RouteStops { get; set; }
        DbSet<SeatTemplate> SeatTemplates { get; set; }
        DbSet<CmsConfig> CmsConfigs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
