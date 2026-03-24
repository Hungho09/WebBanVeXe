using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<BusType> BusTypes { get; set; }
        public DbSet<StopPoint> StopPoints { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<SeatTemplate> SeatTemplates { get; set; }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await Database.BeginTransactionAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.FullName).HasMaxLength(150);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.HasIndex(e => e.Email).IsUnique();

                // Seed Data
                entity.HasData(new User
                {
                    Id = System.Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    UserName = "admin",
                    Email = "admin@vexesystem.com",
                    FullName = "System Administrator",
                    PhoneNumber = "0123456789",
                    PasswordHash = "$2a$11$0nK18Qc7D8N94B3U3P6S/OGfN9f4v.T2H6zH/r4O/C5v.Q/b4XvG6", // Fixed Hash for "Admin@123" to avoid EF pending model changes
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
                });
            });

            // Seed BusTypes
            modelBuilder.Entity<BusType>().HasData(
                new BusType { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Limousine", SeatCount = 9, Description = "VIP Limousine" },
                new BusType { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Giường nằm", SeatCount = 44, Description = "Sleeper Bus Standard" },
                new BusType { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Ghế ngồi", SeatCount = 45, Description = "Standard Seat Bus" }
            );

            modelBuilder.Entity<Route>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Origin).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Destination).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Points).HasMaxLength(500);
                entity.Property(e => e.DistanceKm).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
            });

            // Configure Bus
            modelBuilder.Entity<Bus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlateNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                
                entity.HasOne(e => e.BusType)
                    .WithMany(bt => bt.Buses)
                    .HasForeignKey(e => e.BusTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure BusType
            modelBuilder.Entity<BusType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SeatCount).IsRequired();
            });

            // Configure StopPoint
            modelBuilder.Entity<StopPoint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            });

            // Configure RouteStop
            modelBuilder.Entity<RouteStop>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Route)
                    .WithMany(r => r.RouteStops)
                    .HasForeignKey(e => e.RouteId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.StopPoint)
                    .WithMany(sp => sp.RouteStops)
                    .HasForeignKey(e => e.StopPointId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DepartureTime).IsRequired();
                entity.Property(e => e.ArrivalTime).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.HasOne(e => e.Route).WithMany(r => r.Trips).HasForeignKey(e => e.RouteId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Bus).WithMany(b => b.Trips).HasForeignKey(e => e.BusId).OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Seat
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SeatNumber).IsRequired().HasMaxLength(10);
                entity.HasOne(e => e.Trip).WithMany(t => t.Seats).HasForeignKey(e => e.TripId).OnDelete(DeleteBehavior.Restrict);
            });

            // Configure SeatTemplate
            modelBuilder.Entity<SeatTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SeatNumber).IsRequired().HasMaxLength(10);
            });

            // Configure Booking
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Trip).WithMany().HasForeignKey(e => e.TripId).OnDelete(DeleteBehavior.Restrict);
            });

            // Configure BookingDetail
            modelBuilder.Entity<BookingDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasOne(e => e.Booking)
                    .WithMany(b => b.BookingDetails)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Seat)
                    .WithMany()
                    .HasForeignKey(e => e.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.Property(e => e.TransactionCode).HasMaxLength(100);
                entity.HasOne(e => e.Booking).WithMany().HasForeignKey(e => e.BookingId).OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Invoice
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.HasOne(e => e.Booking).WithMany().HasForeignKey(e => e.BookingId).OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            });

            // Seed Seat Templates
            SeatTemplateSeed.Seed(modelBuilder);
        }
    }
}
