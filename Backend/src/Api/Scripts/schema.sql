USE [WebBanVeXeDB];
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [BusTypes] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [SeatCount] int NOT NULL,
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK_BusTypes] PRIMARY KEY ([Id])
);

CREATE TABLE [CmsConfigs] (
    [Id] int NOT NULL IDENTITY,
    [ConfigKey] nvarchar(100) NOT NULL,
    [ContentJson] nvarchar(max) NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CmsConfigs] PRIMARY KEY ([Id])
);

CREATE TABLE [Routes] (
    [Id] uniqueidentifier NOT NULL,
    [Origin] nvarchar(200) NOT NULL,
    [Destination] nvarchar(200) NOT NULL,
    [Points] nvarchar(500) NULL,
    [DistanceKm] int NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Routes] PRIMARY KEY ([Id])
);

CREATE TABLE [StopPoints] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Address] nvarchar(500) NOT NULL,
    [Latitude] float NULL,
    [Longitude] float NULL,
    [IsPickup] bit NOT NULL,
    [IsDropoff] bit NOT NULL,
    [ProvinceName] nvarchar(100) NULL,
    [MapLink] nvarchar(1000) NULL,
    [IsDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [Badge] nvarchar(max) NULL,
    CONSTRAINT [PK_StopPoints] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [UserName] nvarchar(50) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [FullName] nvarchar(150) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [Buses] (
    [Id] uniqueidentifier NOT NULL,
    [PlateNumber] nvarchar(50) NOT NULL,
    [CompanyName] nvarchar(200) NOT NULL,
    [ImageUrl] nvarchar(500) NULL,
    [BusTypeId] uniqueidentifier NOT NULL,
    [SeatCount] int NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Buses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Buses_BusTypes_BusTypeId] FOREIGN KEY ([BusTypeId]) REFERENCES [BusTypes] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [SeatTemplates] (
    [Id] uniqueidentifier NOT NULL,
    [BusTypeId] uniqueidentifier NOT NULL,
    [SeatNumber] nvarchar(10) NOT NULL,
    [RowNumber] int NOT NULL,
    [ColumnNumber] int NOT NULL,
    [Floor] int NOT NULL,
    [Type] int NOT NULL,
    CONSTRAINT [PK_SeatTemplates] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SeatTemplates_BusTypes_BusTypeId] FOREIGN KEY ([BusTypeId]) REFERENCES [BusTypes] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [RouteStops] (
    [Id] uniqueidentifier NOT NULL,
    [RouteId] uniqueidentifier NOT NULL,
    [StopPointId] uniqueidentifier NOT NULL,
    [OffsetMinutes] int NOT NULL,
    [DistanceFromOriginKm] float NOT NULL,
    [OrderIndex] int NOT NULL,
    CONSTRAINT [PK_RouteStops] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RouteStops_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RouteStops_StopPoints_StopPointId] FOREIGN KEY ([StopPointId]) REFERENCES [StopPoints] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Trips] (
    [Id] uniqueidentifier NOT NULL,
    [RouteId] uniqueidentifier NOT NULL,
    [BusId] uniqueidentifier NOT NULL,
    [DepartureTime] datetime2 NOT NULL,
    [ArrivalTime] datetime2 NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Trips] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Trips_Buses_BusId] FOREIGN KEY ([BusId]) REFERENCES [Buses] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Trips_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Bookings] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [TripId] uniqueidentifier NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [BookingStatus] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [PickupPointId] uniqueidentifier NULL,
    [DropoffPointId] uniqueidentifier NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Bookings_StopPoints_DropoffPointId] FOREIGN KEY ([DropoffPointId]) REFERENCES [StopPoints] ([Id]),
    CONSTRAINT [FK_Bookings_StopPoints_PickupPointId] FOREIGN KEY ([PickupPointId]) REFERENCES [StopPoints] ([Id]),
    CONSTRAINT [FK_Bookings_Trips_TripId] FOREIGN KEY ([TripId]) REFERENCES [Trips] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Bookings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Seats] (
    [Id] uniqueidentifier NOT NULL,
    [TripId] uniqueidentifier NOT NULL,
    [SeatNumber] nvarchar(10) NOT NULL,
    [RowNumber] int NOT NULL,
    [ColumnNumber] int NOT NULL,
    [Floor] int NOT NULL,
    [Type] int NOT NULL,
    [Status] int NOT NULL,
    [LockExpirationTime] datetime2 NULL,
    [LockedByUserId] uniqueidentifier NULL,
    [RowVersion] rowversion,
    CONSTRAINT [PK_Seats] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Seats_Trips_TripId] FOREIGN KEY ([TripId]) REFERENCES [Trips] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Seats_TripId] ON [Seats] ([TripId]);
CREATE INDEX [IX_Trips_BusId] ON [Trips] ([BusId]);
CREATE INDEX [IX_Trips_RouteId] ON [Trips] ([RouteId]);

COMMIT;
GO
