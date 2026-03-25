using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SeatCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CmsConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConfigKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContentJson = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CmsConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Origin = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Destination = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Points = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DistanceKm = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StopPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    IsPickup = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDropoff = table.Column<bool>(type: "INTEGER", nullable: false),
                    Badge = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlateNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BusTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SeatCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buses_BusTypes_BusTypeId",
                        column: x => x.BusTypeId,
                        principalTable: "BusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeatTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BusTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SeatNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    RowNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ColumnNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Floor = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeatTemplates_BusTypes_BusTypeId",
                        column: x => x.BusTypeId,
                        principalTable: "BusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RouteStops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RouteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StopPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OffsetMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    DistanceFromOriginKm = table.Column<double>(type: "REAL", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteStops_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteStops_StopPoints_StopPointId",
                        column: x => x.StopPointId,
                        principalTable: "StopPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    IsSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RouteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BusId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trips_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trips_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TripId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    BookingStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PickupPointId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DropoffPointId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_StopPoints_DropoffPointId",
                        column: x => x.DropoffPointId,
                        principalTable: "StopPoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_StopPoints_PickupPointId",
                        column: x => x.PickupPointId,
                        principalTable: "StopPoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TripId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SeatNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    RowNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ColumnNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Floor = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LockExpirationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LockedByUserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seats_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BookingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerEmail = table.Column<string>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BookingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BookingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SeatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Seats_SeatId",
                        column: x => x.SeatId,
                        principalTable: "Seats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "BusTypes",
                columns: new[] { "Id", "Description", "Name", "SeatCount" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222222"), "VIP Limousine", "Limousine", 9 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Sleeper Bus Standard", "Giường nằm", 44 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Standard Seat Bus", "Ghế ngồi", 45 }
                });

            migrationBuilder.InsertData(
                table: "CmsConfigs",
                columns: new[] { "Id", "ConfigKey", "ContentJson", "UpdatedAt" },
                values: new object[] { 1, "homepage_v1", "{}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "PhoneNumber", "Role", "UserName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@vexesystem.com", "System Administrator", true, "$2a$11$0nK18Qc7D8N94B3U3P6S/OGfN9f4v.T2H6zH/r4O/C5v.Q/b4XvG6", "0123456789", "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "Buses",
                columns: new[] { "Id", "BusTypeId", "CompanyName", "ImageUrl", "PlateNumber", "SeatCount", "Status" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("33333333-3333-3333-3333-333333333333"), "Phương Trang (FUTA)", null, "51B-123.45", 44, 2 },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new Guid("22222222-2222-2222-2222-222222222222"), "Thành Bưởi", null, "51B-678.90", 9, 1 }
                });

            migrationBuilder.InsertData(
                table: "SeatTemplates",
                columns: new[] { "Id", "BusTypeId", "ColumnNumber", "Floor", "RowNumber", "SeatNumber", "Type" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 1, 1, "A01", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 1, 1, "A02", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 1, 1, "A03", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 1, 2, "A04", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 1, 2, "A05", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 1, 2, "A06", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000007"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 1, 3, "A07", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000008"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 1, 3, "A08", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 1, 3, "A09", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000010"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 1, 4, "A10", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000011"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 1, 4, "A11", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000012"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 1, 4, "A12", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 1, 5, "A13", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000014"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 1, 5, "A14", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000015"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 1, 5, "A15", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000016"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 1, 6, "A16", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000017"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 1, 6, "A17", 0 },
                    { new Guid("10000000-0000-0000-0000-000000000018"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 1, 6, "A18", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000019"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 2, 1, "B01", 1 },
                    { new Guid("20000000-0000-0000-0000-000000000020"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 2, 1, "B02", 1 },
                    { new Guid("20000000-0000-0000-0000-000000000021"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 2, 1, "B03", 1 },
                    { new Guid("20000000-0000-0000-0000-000000000022"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 2, 2, "B04", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000023"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 2, 2, "B05", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000024"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 2, 2, "B06", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000025"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 2, 3, "B07", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000026"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 2, 3, "B08", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000027"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 2, 3, "B09", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000028"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 2, 4, "B10", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000029"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 2, 4, "B11", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000030"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 2, 4, "B12", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000031"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 2, 5, "B13", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000032"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 2, 5, "B14", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000033"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 2, 5, "B15", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000034"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 2, 6, "B16", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000035"), new Guid("33333333-3333-3333-3333-333333333333"), 2, 2, 6, "B17", 0 },
                    { new Guid("20000000-0000-0000-0000-000000000036"), new Guid("33333333-3333-3333-3333-333333333333"), 3, 2, 6, "B18", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000001"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 1, 1, "S01", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000002"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 1, 1, "S02", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000003"), new Guid("44444444-4444-4444-4444-444444444444"), 3, 1, 1, "S03", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000004"), new Guid("44444444-4444-4444-4444-444444444444"), 4, 1, 1, "S04", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000005"), new Guid("44444444-4444-4444-4444-444444444444"), 5, 1, 1, "S05", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000006"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 1, 2, "S06", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000007"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 1, 2, "S07", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000008"), new Guid("44444444-4444-4444-4444-444444444444"), 3, 1, 2, "S08", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000009"), new Guid("44444444-4444-4444-4444-444444444444"), 4, 1, 2, "S09", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000010"), new Guid("44444444-4444-4444-4444-444444444444"), 5, 1, 2, "S10", 1 },
                    { new Guid("45000000-0000-0000-0000-000000000011"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 1, 3, "S11", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000012"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 1, 3, "S12", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000013"), new Guid("44444444-4444-4444-4444-444444444444"), 3, 1, 3, "S13", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000014"), new Guid("44444444-4444-4444-4444-444444444444"), 4, 1, 3, "S14", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000015"), new Guid("44444444-4444-4444-4444-444444444444"), 5, 1, 3, "S15", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000016"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 1, 4, "S16", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000017"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 1, 4, "S17", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000018"), new Guid("44444444-4444-4444-4444-444444444444"), 3, 1, 4, "S18", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000019"), new Guid("44444444-4444-4444-4444-444444444444"), 4, 1, 4, "S19", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000020"), new Guid("44444444-4444-4444-4444-444444444444"), 5, 1, 4, "S20", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000021"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 1, 5, "S21", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000022"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 1, 5, "S22", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000023"), new Guid("44444444-4444-4444-4444-444444444444"), 3, 1, 5, "S23", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000024"), new Guid("44444444-4444-4444-4444-444444444444"), 4, 1, 5, "S24", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000025"), new Guid("44444444-4444-4444-4444-444444444444"), 5, 1, 5, "S25", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000026"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 1, 6, "S26", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000027"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 1, 6, "S27", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000028"), new Guid("44444444-4444-4444-4444-444444444444"), 3, 1, 6, "S28", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000029"), new Guid("44444444-4444-4444-4444-444444444444"), 4, 1, 6, "S29", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000030"), new Guid("44444444-4444-4444-4444-444444444444"), 5, 1, 6, "S30", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000031"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 1, 7, "S31", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000032"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 1, 7, "S32", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000033"), new Guid("44444444-4444-4444-4444-444444444444"), 3, 1, 7, "S33", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000034"), new Guid("44444444-4444-4444-4444-444444444444"), 4, 1, 7, "S34", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000035"), new Guid("44444444-4444-4444-4444-444444444444"), 5, 1, 7, "S35", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000036"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 1, 8, "S36", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000037"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 1, 8, "S37", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000038"), new Guid("44444444-4444-4444-4444-444444444444"), 3, 1, 8, "S38", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000039"), new Guid("44444444-4444-4444-4444-444444444444"), 4, 1, 8, "S39", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000040"), new Guid("44444444-4444-4444-4444-444444444444"), 5, 1, 8, "S40", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000041"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 1, 9, "S41", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000042"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 1, 9, "S42", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000043"), new Guid("44444444-4444-4444-4444-444444444444"), 3, 1, 9, "S43", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000044"), new Guid("44444444-4444-4444-4444-444444444444"), 4, 1, 9, "S44", 0 },
                    { new Guid("45000000-0000-0000-0000-000000000045"), new Guid("44444444-4444-4444-4444-444444444444"), 5, 1, 9, "S45", 0 },
                    { new Guid("90000000-0000-0000-0000-000000000001"), new Guid("22222222-2222-2222-2222-222222222222"), 1, 1, 1, "L01", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000002"), new Guid("22222222-2222-2222-2222-222222222222"), 2, 1, 1, "L02", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000003"), new Guid("22222222-2222-2222-2222-222222222222"), 3, 1, 1, "L03", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000004"), new Guid("22222222-2222-2222-2222-222222222222"), 1, 1, 2, "L04", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000005"), new Guid("22222222-2222-2222-2222-222222222222"), 2, 1, 2, "L05", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000006"), new Guid("22222222-2222-2222-2222-222222222222"), 3, 1, 2, "L06", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000007"), new Guid("22222222-2222-2222-2222-222222222222"), 1, 1, 3, "L07", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000008"), new Guid("22222222-2222-2222-2222-222222222222"), 2, 1, 3, "L08", 1 },
                    { new Guid("90000000-0000-0000-0000-000000000009"), new Guid("22222222-2222-2222-2222-222222222222"), 3, 1, 3, "L09", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_BookingId",
                table: "BookingDetails",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_SeatId",
                table: "BookingDetails",
                column: "SeatId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DropoffPointId",
                table: "Bookings",
                column: "DropoffPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PickupPointId",
                table: "Bookings",
                column: "PickupPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TripId",
                table: "Bookings",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_BusTypeId",
                table: "Buses",
                column: "BusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CmsConfigs_ConfigKey",
                table: "CmsConfigs",
                column: "ConfigKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BookingId",
                table: "Invoices",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId",
                table: "RouteStops",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_StopPointId",
                table: "RouteStops",
                column: "StopPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_TripId",
                table: "Seats",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatTemplates_BusTypeId",
                table: "SeatTemplates",
                column: "BusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_BusId",
                table: "Trips",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_RouteId",
                table: "Trips",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingDetails");

            migrationBuilder.DropTable(
                name: "CmsConfigs");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RouteStops");

            migrationBuilder.DropTable(
                name: "SeatTemplates");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "StopPoints");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Buses");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "BusTypes");
        }
    }
}
