IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WebBanVeXeDB')
BEGIN
    CREATE DATABASE [WebBanVeXeDB];
END
GO

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

CREATE TABLE [Notifications] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [IsSent] bit NOT NULL,
    [SentAt] datetime2 NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
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

CREATE TABLE [Invoices] (
    [Id] uniqueidentifier NOT NULL,
    [InvoiceNumber] nvarchar(50) NOT NULL,
    [BookingId] uniqueidentifier NOT NULL,
    [CustomerName] nvarchar(max) NOT NULL,
    [CustomerEmail] nvarchar(max) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Invoices_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Payments] (
    [Id] uniqueidentifier NOT NULL,
    [BookingId] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [PaymentMethod] nvarchar(50) NOT NULL,
    [PaymentStatus] int NOT NULL,
    [TransactionCode] nvarchar(100) NOT NULL,
    [PaidAt] datetime2 NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [BookingDetails] (
    [Id] uniqueidentifier NOT NULL,
    [BookingId] uniqueidentifier NOT NULL,
    [SeatId] uniqueidentifier NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_BookingDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BookingDetails_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BookingDetails_Seats_SeatId] FOREIGN KEY ([SeatId]) REFERENCES [Seats] ([Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name', N'SeatCount') AND [object_id] = OBJECT_ID(N'[BusTypes]'))
    SET IDENTITY_INSERT [BusTypes] ON;
INSERT INTO [BusTypes] ([Id], [Description], [Name], [SeatCount])
VALUES ('22222222-2222-2222-2222-222222222222', N'VIP Limousine', N'Limousine', 9),
('33333333-3333-3333-3333-333333333333', N'Sleeper Bus Standard', N'Giường nằm', 44),
('44444444-4444-4444-4444-444444444444', N'Standard Seat Bus', N'Ghế ngồi', 45);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name', N'SeatCount') AND [object_id] = OBJECT_ID(N'[BusTypes]'))
    SET IDENTITY_INSERT [BusTypes] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConfigKey', N'ContentJson', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[CmsConfigs]'))
    SET IDENTITY_INSERT [CmsConfigs] ON;
INSERT INTO [CmsConfigs] ([Id], [ConfigKey], [ContentJson], [UpdatedAt])
VALUES (1, N'homepage_v1', N'{}', '2026-01-01T00:00:00.0000000Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConfigKey', N'ContentJson', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[CmsConfigs]'))
    SET IDENTITY_INSERT [CmsConfigs] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'FullName', N'IsActive', N'PasswordHash', N'PhoneNumber', N'Role', N'UserName') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [CreatedAt], [Email], [FullName], [IsActive], [PasswordHash], [PhoneNumber], [Role], [UserName])
VALUES ('11111111-1111-1111-1111-111111111111', '2026-01-01T00:00:00.0000000Z', N'admin@vexesystem.com', N'System Administrator', CAST(1 AS bit), N'$2a$11$.N14FFcQMoCcN3OZvl3DkOMgSok1ocYfR2qyWMxVmlpuqUxxl20la', N'0123456789', N'Admin', N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'FullName', N'IsActive', N'PasswordHash', N'PhoneNumber', N'Role', N'UserName') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BusTypeId', N'CompanyName', N'ImageUrl', N'PlateNumber', N'SeatCount', N'Status') AND [object_id] = OBJECT_ID(N'[Buses]'))
    SET IDENTITY_INSERT [Buses] ON;
INSERT INTO [Buses] ([Id], [BusTypeId], [CompanyName], [ImageUrl], [PlateNumber], [SeatCount], [Status])
VALUES ('55555555-5555-5555-5555-555555555555', '33333333-3333-3333-3333-333333333333', N'Phương Trang (FUTA)', NULL, N'51B-123.45', 44, 2),
('66666666-6666-6666-6666-666666666666', '22222222-2222-2222-2222-222222222222', N'Thành Bưởi', NULL, N'51B-678.90', 9, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BusTypeId', N'CompanyName', N'ImageUrl', N'PlateNumber', N'SeatCount', N'Status') AND [object_id] = OBJECT_ID(N'[Buses]'))
    SET IDENTITY_INSERT [Buses] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BusTypeId', N'ColumnNumber', N'Floor', N'RowNumber', N'SeatNumber', N'Type') AND [object_id] = OBJECT_ID(N'[SeatTemplates]'))
    SET IDENTITY_INSERT [SeatTemplates] ON;
INSERT INTO [SeatTemplates] ([Id], [BusTypeId], [ColumnNumber], [Floor], [RowNumber], [SeatNumber], [Type])
VALUES ('10000000-0000-0000-0000-000000000001', '33333333-3333-3333-3333-333333333333', 1, 1, 1, N'A01', 1),
('10000000-0000-0000-0000-000000000002', '33333333-3333-3333-3333-333333333333', 2, 1, 1, N'A02', 1),
('10000000-0000-0000-0000-000000000003', '33333333-3333-3333-3333-333333333333', 3, 1, 1, N'A03', 1),
('10000000-0000-0000-0000-000000000004', '33333333-3333-3333-3333-333333333333', 1, 1, 2, N'A04', 0),
('10000000-0000-0000-0000-000000000005', '33333333-3333-3333-3333-333333333333', 2, 1, 2, N'A05', 0),
('10000000-0000-0000-0000-000000000006', '33333333-3333-3333-3333-333333333333', 3, 1, 2, N'A06', 0),
('10000000-0000-0000-0000-000000000007', '33333333-3333-3333-3333-333333333333', 1, 1, 3, N'A07', 0),
('10000000-0000-0000-0000-000000000008', '33333333-3333-3333-3333-333333333333', 2, 1, 3, N'A08', 0),
('10000000-0000-0000-0000-000000000009', '33333333-3333-3333-3333-333333333333', 3, 1, 3, N'A09', 0),
('10000000-0000-0000-0000-000000000010', '33333333-3333-3333-3333-333333333333', 1, 1, 4, N'A10', 0),
('10000000-0000-0000-0000-000000000011', '33333333-3333-3333-3333-333333333333', 2, 1, 4, N'A11', 0),
('10000000-0000-0000-0000-000000000012', '33333333-3333-3333-3333-333333333333', 3, 1, 4, N'A12', 0),
('10000000-0000-0000-0000-000000000013', '33333333-3333-3333-3333-333333333333', 1, 1, 5, N'A13', 0),
('10000000-0000-0000-0000-000000000014', '33333333-3333-3333-3333-333333333333', 2, 1, 5, N'A14', 0),
('10000000-0000-0000-0000-000000000015', '33333333-3333-3333-3333-333333333333', 3, 1, 5, N'A15', 0),
('10000000-0000-0000-0000-000000000016', '33333333-3333-3333-3333-333333333333', 1, 1, 6, N'A16', 0),
('10000000-0000-0000-0000-000000000017', '33333333-3333-3333-3333-333333333333', 2, 1, 6, N'A17', 0),
('10000000-0000-0000-0000-000000000018', '33333333-3333-3333-3333-333333333333', 3, 1, 6, N'A18', 0),
('20000000-0000-0000-0000-000000000019', '33333333-3333-3333-3333-333333333333', 1, 2, 1, N'B01', 1),
('20000000-0000-0000-0000-000000000020', '33333333-3333-3333-3333-333333333333', 2, 2, 1, N'B02', 1),
('20000000-0000-0000-0000-000000000021', '33333333-3333-3333-3333-333333333333', 3, 2, 1, N'B03', 1),
('20000000-0000-0000-0000-000000000022', '33333333-3333-3333-3333-333333333333', 1, 2, 2, N'B04', 0),
('20000000-0000-0000-0000-000000000023', '33333333-3333-3333-3333-333333333333', 2, 2, 2, N'B05', 0),
('20000000-0000-0000-0000-000000000024', '33333333-3333-3333-3333-333333333333', 3, 2, 2, N'B06', 0),
('20000000-0000-0000-0000-000000000025', '33333333-3333-3333-3333-333333333333', 1, 2, 3, N'B07', 0),
('20000000-0000-0000-0000-000000000026', '33333333-3333-3333-3333-333333333333', 2, 2, 3, N'B08', 0),
('20000000-0000-0000-0000-000000000027', '33333333-3333-3333-3333-333333333333', 3, 2, 3, N'B09', 0),
('20000000-0000-0000-0000-000000000028', '33333333-3333-3333-3333-333333333333', 1, 2, 4, N'B10', 0),
('20000000-0000-0000-0000-000000000029', '33333333-3333-3333-3333-333333333333', 2, 2, 4, N'B11', 0),
('20000000-0000-0000-0000-000000000030', '33333333-3333-3333-3333-333333333333', 3, 2, 4, N'B12', 0),
('20000000-0000-0000-0000-000000000031', '33333333-3333-3333-3333-333333333333', 1, 2, 5, N'B13', 0),
('20000000-0000-0000-0000-000000000032', '33333333-3333-3333-3333-333333333333', 2, 2, 5, N'B14', 0),
('20000000-0000-0000-0000-000000000033', '33333333-3333-3333-3333-333333333333', 3, 2, 5, N'B15', 0),
('20000000-0000-0000-0000-000000000034', '33333333-3333-3333-3333-333333333333', 1, 2, 6, N'B16', 0),
('20000000-0000-0000-0000-000000000035', '33333333-3333-3333-3333-333333333333', 2, 2, 6, N'B17', 0),
('20000000-0000-0000-0000-000000000036', '33333333-3333-3333-3333-333333333333', 3, 2, 6, N'B18', 0),
('45000000-0000-0000-0000-000000000001', '44444444-4444-4444-4444-444444444444', 1, 1, 1, N'S01', 1),
('45000000-0000-0000-0000-000000000002', '44444444-4444-4444-4444-444444444444', 2, 1, 1, N'S02', 1),
('45000000-0000-0000-0000-000000000003', '44444444-4444-4444-4444-444444444444', 3, 1, 1, N'S03', 1),
('45000000-0000-0000-0000-000000000004', '44444444-4444-4444-4444-444444444444', 4, 1, 1, N'S04', 1),
('45000000-0000-0000-0000-000000000005', '44444444-4444-4444-4444-444444444444', 5, 1, 1, N'S05', 1),
('45000000-0000-0000-0000-000000000006', '44444444-4444-4444-4444-444444444444', 1, 1, 2, N'S06', 1);
INSERT INTO [SeatTemplates] ([Id], [BusTypeId], [ColumnNumber], [Floor], [RowNumber], [SeatNumber], [Type])
VALUES ('45000000-0000-0000-0000-000000000007', '44444444-4444-4444-4444-444444444444', 2, 1, 2, N'S07', 1),
('45000000-0000-0000-0000-000000000008', '44444444-4444-4444-4444-444444444444', 3, 1, 2, N'S08', 1),
('45000000-0000-0000-0000-000000000009', '44444444-4444-4444-4444-444444444444', 4, 1, 2, N'S09', 1),
('45000000-0000-0000-0000-000000000010', '44444444-4444-4444-4444-444444444444', 5, 1, 2, N'S10', 1),
('45000000-0000-0000-0000-000000000011', '44444444-4444-4444-4444-444444444444', 1, 1, 3, N'S11', 0),
('45000000-0000-0000-0000-000000000012', '44444444-4444-4444-4444-444444444444', 2, 1, 3, N'S12', 0),
('45000000-0000-0000-0000-000000000013', '44444444-4444-4444-4444-444444444444', 3, 1, 3, N'S13', 0),
('45000000-0000-0000-0000-000000000014', '44444444-4444-4444-4444-444444444444', 4, 1, 3, N'S14', 0),
('45000000-0000-0000-0000-000000000015', '44444444-4444-4444-4444-444444444444', 5, 1, 3, N'S15', 0),
('45000000-0000-0000-0000-000000000016', '44444444-4444-4444-4444-444444444444', 1, 1, 4, N'S16', 0),
('45000000-0000-0000-0000-000000000017', '44444444-4444-4444-4444-444444444444', 2, 1, 4, N'S17', 0),
('45000000-0000-0000-0000-000000000018', '44444444-4444-4444-4444-444444444444', 3, 1, 4, N'S18', 0),
('45000000-0000-0000-0000-000000000019', '44444444-4444-4444-4444-444444444444', 4, 1, 4, N'S19', 0),
('45000000-0000-0000-0000-000000000020', '44444444-4444-4444-4444-444444444444', 5, 1, 4, N'S20', 0),
('45000000-0000-0000-0000-000000000021', '44444444-4444-4444-4444-444444444444', 1, 1, 5, N'S21', 0),
('45000000-0000-0000-0000-000000000022', '44444444-4444-4444-4444-444444444444', 2, 1, 5, N'S22', 0),
('45000000-0000-0000-0000-000000000023', '44444444-4444-4444-4444-444444444444', 3, 1, 5, N'S23', 0),
('45000000-0000-0000-0000-000000000024', '44444444-4444-4444-4444-444444444444', 4, 1, 5, N'S24', 0),
('45000000-0000-0000-0000-000000000025', '44444444-4444-4444-4444-444444444444', 5, 1, 5, N'S25', 0),
('45000000-0000-0000-0000-000000000026', '44444444-4444-4444-4444-444444444444', 1, 1, 6, N'S26', 0),
('45000000-0000-0000-0000-000000000027', '44444444-4444-4444-4444-444444444444', 2, 1, 6, N'S27', 0),
('45000000-0000-0000-0000-000000000028', '44444444-4444-4444-4444-444444444444', 3, 1, 6, N'S28', 0),
('45000000-0000-0000-0000-000000000029', '44444444-4444-4444-4444-444444444444', 4, 1, 6, N'S29', 0),
('45000000-0000-0000-0000-000000000030', '44444444-4444-4444-4444-444444444444', 5, 1, 6, N'S30', 0),
('45000000-0000-0000-0000-000000000031', '44444444-4444-4444-4444-444444444444', 1, 1, 7, N'S31', 0),
('45000000-0000-0000-0000-000000000032', '44444444-4444-4444-4444-444444444444', 2, 1, 7, N'S32', 0),
('45000000-0000-0000-0000-000000000033', '44444444-4444-4444-4444-444444444444', 3, 1, 7, N'S33', 0),
('45000000-0000-0000-0000-000000000034', '44444444-4444-4444-4444-444444444444', 4, 1, 7, N'S34', 0),
('45000000-0000-0000-0000-000000000035', '44444444-4444-4444-4444-444444444444', 5, 1, 7, N'S35', 0),
('45000000-0000-0000-0000-000000000036', '44444444-4444-4444-4444-444444444444', 1, 1, 8, N'S36', 0),
('45000000-0000-0000-0000-000000000037', '44444444-4444-4444-4444-444444444444', 2, 1, 8, N'S37', 0),
('45000000-0000-0000-0000-000000000038', '44444444-4444-4444-4444-444444444444', 3, 1, 8, N'S38', 0),
('45000000-0000-0000-0000-000000000039', '44444444-4444-4444-4444-444444444444', 4, 1, 8, N'S39', 0),
('45000000-0000-0000-0000-000000000040', '44444444-4444-4444-4444-444444444444', 5, 1, 8, N'S40', 0),
('45000000-0000-0000-0000-000000000041', '44444444-4444-4444-4444-444444444444', 1, 1, 9, N'S41', 0),
('45000000-0000-0000-0000-000000000042', '44444444-4444-4444-4444-444444444444', 2, 1, 9, N'S42', 0),
('45000000-0000-0000-0000-000000000043', '44444444-4444-4444-4444-444444444444', 3, 1, 9, N'S43', 0),
('45000000-0000-0000-0000-000000000044', '44444444-4444-4444-4444-444444444444', 4, 1, 9, N'S44', 0),
('45000000-0000-0000-0000-000000000045', '44444444-4444-4444-4444-444444444444', 5, 1, 9, N'S45', 0),
('90000000-0000-0000-0000-000000000001', '22222222-2222-2222-2222-222222222222', 1, 1, 1, N'L01', 1),
('90000000-0000-0000-0000-000000000002', '22222222-2222-2222-2222-222222222222', 2, 1, 1, N'L02', 1),
('90000000-0000-0000-0000-000000000003', '22222222-2222-2222-2222-222222222222', 3, 1, 1, N'L03', 1);
INSERT INTO [SeatTemplates] ([Id], [BusTypeId], [ColumnNumber], [Floor], [RowNumber], [SeatNumber], [Type])
VALUES ('90000000-0000-0000-0000-000000000004', '22222222-2222-2222-2222-222222222222', 1, 1, 2, N'L04', 1),
('90000000-0000-0000-0000-000000000005', '22222222-2222-2222-2222-222222222222', 2, 1, 2, N'L05', 1),
('90000000-0000-0000-0000-000000000006', '22222222-2222-2222-2222-222222222222', 3, 1, 2, N'L06', 1),
('90000000-0000-0000-0000-000000000007', '22222222-2222-2222-2222-222222222222', 1, 1, 3, N'L07', 1),
('90000000-0000-0000-0000-000000000008', '22222222-2222-2222-2222-222222222222', 2, 1, 3, N'L08', 1),
('90000000-0000-0000-0000-000000000009', '22222222-2222-2222-2222-222222222222', 3, 1, 3, N'L09', 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BusTypeId', N'ColumnNumber', N'Floor', N'RowNumber', N'SeatNumber', N'Type') AND [object_id] = OBJECT_ID(N'[SeatTemplates]'))
    SET IDENTITY_INSERT [SeatTemplates] OFF;

CREATE INDEX [IX_BookingDetails_BookingId] ON [BookingDetails] ([BookingId]);

CREATE INDEX [IX_BookingDetails_SeatId] ON [BookingDetails] ([SeatId]);

CREATE INDEX [IX_Bookings_DropoffPointId] ON [Bookings] ([DropoffPointId]);

CREATE INDEX [IX_Bookings_PickupPointId] ON [Bookings] ([PickupPointId]);

CREATE INDEX [IX_Bookings_TripId] ON [Bookings] ([TripId]);

CREATE INDEX [IX_Bookings_UserId] ON [Bookings] ([UserId]);

CREATE INDEX [IX_Buses_BusTypeId] ON [Buses] ([BusTypeId]);

CREATE UNIQUE INDEX [IX_CmsConfigs_ConfigKey] ON [CmsConfigs] ([ConfigKey]);

CREATE INDEX [IX_Invoices_BookingId] ON [Invoices] ([BookingId]);

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);

CREATE INDEX [IX_Payments_BookingId] ON [Payments] ([BookingId]);

CREATE INDEX [IX_RouteStops_RouteId] ON [RouteStops] ([RouteId]);

CREATE INDEX [IX_RouteStops_StopPointId] ON [RouteStops] ([StopPointId]);

CREATE INDEX [IX_Seats_TripId] ON [Seats] ([TripId]);

CREATE INDEX [IX_SeatTemplates_BusTypeId] ON [SeatTemplates] ([BusTypeId]);

CREATE INDEX [IX_Trips_BusId] ON [Trips] ([BusId]);

CREATE INDEX [IX_Trips_RouteId] ON [Trips] ([RouteId]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260325080058_InitialSqlServer', N'9.0.0');

COMMIT;
GO

-- Insert missing BusTypes
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '11000000-0000-0000-0000-000000000016')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('11000000-0000-0000-0000-000000000016', N'Xe ghế ngồi', 16, 'Standard Normal Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '11000000-0000-0000-0000-000000000029')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('11000000-0000-0000-0000-000000000029', N'Xe ghế ngồi', 29, 'Standard Normal Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '22000000-0000-0000-0000-000000000011')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('22000000-0000-0000-0000-000000000011', N'Xe Limousine', 11, 'VIP Limousine Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '22000000-0000-0000-0000-000000000016')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('22000000-0000-0000-0000-000000000016', N'Xe Limousine', 16, 'VIP Limousine Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '22000000-0000-0000-0000-000000000019')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('22000000-0000-0000-0000-000000000019', N'Xe Limousine', 19, 'VIP Limousine Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '33000000-0000-0000-0000-000000000034')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('33000000-0000-0000-0000-000000000034', N'Xe giường nằm', 34, 'Standard Sleeper');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '55000000-0000-0000-0000-000000000020')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('55000000-0000-0000-0000-000000000020', N'Xe giường phòng Cabin', 20, 'Cabin Single');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '55000000-0000-0000-0000-000000000024')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('55000000-0000-0000-0000-000000000024', N'Xe giường phòng Cabin', 24, 'Cabin Single');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '66000000-0000-0000-0000-000000000022')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('66000000-0000-0000-0000-000000000022', N'Xe giường phòng Cabin (Đôi)', 22, 'Cabin Double');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '66000000-0000-0000-0000-000000000024')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('66000000-0000-0000-0000-000000000024', N'Xe giường phòng Cabin (Đôi)', 24, 'Cabin Double');

-- Now generate SeatTemplates for the new types that don't have templates yet
-- Helper: Generate templates for a BusType by given params
-- Xe ghe ngoi 16 cho (4 cols, 1 floor)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '11000000-0000-0000-0000-000000000016')
BEGIN
    DECLARE @i16 INT = 1, @f16 INT = 1, @r16 INT = 1, @c16 INT = 1, @fc16 INT = 0;
    WHILE @i16 <= 16
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('11160000-0000-0000-0000-', RIGHT('000000000000'+CAST(@i16 AS VARCHAR),12)),
                '11000000-0000-0000-0000-000000000016',
                'S' + RIGHT('00'+CAST(@i16 AS VARCHAR),2), @r16, @c16, 1,
                CASE WHEN @r16 = 1 THEN 1 ELSE 0 END);
        SET @c16 = @c16 + 1;
        IF @c16 > 4 BEGIN SET @c16 = 1; SET @r16 = @r16 + 1; END
        SET @i16 = @i16 + 1;
    END
END;

-- Xe ghe ngoi 29 cho (4 cols, 1 floor)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '11000000-0000-0000-0000-000000000029')
BEGIN
    DECLARE @i29 INT = 1, @r29 INT = 1, @c29 INT = 1;
    WHILE @i29 <= 29
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('11290000-0000-0000-0000-', RIGHT('000000000000'+CAST(@i29 AS VARCHAR),12)),
                '11000000-0000-0000-0000-000000000029',
                'S' + RIGHT('00'+CAST(@i29 AS VARCHAR),2), @r29, @c29, 1,
                CASE WHEN @r29 = 1 THEN 1 ELSE 0 END);
        SET @c29 = @c29 + 1;
        IF @c29 > 4 BEGIN SET @c29 = 1; SET @r29 = @r29 + 1; END
        SET @i29 = @i29 + 1;
    END
END;

-- Xe Limousine 11 cho (3 cols, 1 floor)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '22000000-0000-0000-0000-000000000011')
BEGIN
    DECLARE @iL11 INT = 1, @rL11 INT = 1, @cL11 INT = 1;
    WHILE @iL11 <= 11
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('22110000-0000-0000-0000-', RIGHT('000000000000'+CAST(@iL11 AS VARCHAR),12)),
                '22000000-0000-0000-0000-000000000011',
                'L' + RIGHT('00'+CAST(@iL11 AS VARCHAR),2), @rL11, @cL11, 1, 1);
        SET @cL11 = @cL11 + 1;
        IF @cL11 > 3 BEGIN SET @cL11 = 1; SET @rL11 = @rL11 + 1; END
        SET @iL11 = @iL11 + 1;
    END
END;

-- Xe Limousine 16 cho (4 cols, 1 floor)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '22000000-0000-0000-0000-000000000016')
BEGIN
    DECLARE @iL16 INT = 1, @rL16 INT = 1, @cL16 INT = 1;
    WHILE @iL16 <= 16
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('22160000-0000-0000-0000-', RIGHT('000000000000'+CAST(@iL16 AS VARCHAR),12)),
                '22000000-0000-0000-0000-000000000016',
                'L' + RIGHT('00'+CAST(@iL16 AS VARCHAR),2), @rL16, @cL16, 1, 1);
        SET @cL16 = @cL16 + 1;
        IF @cL16 > 4 BEGIN SET @cL16 = 1; SET @rL16 = @rL16 + 1; END
        SET @iL16 = @iL16 + 1;
    END
END;

-- Xe Limousine 19 cho (4 cols, 1 floor)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '22000000-0000-0000-0000-000000000019')
BEGIN
    DECLARE @iL19 INT = 1, @rL19 INT = 1, @cL19 INT = 1;
    WHILE @iL19 <= 19
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('22190000-0000-0000-0000-', RIGHT('000000000000'+CAST(@iL19 AS VARCHAR),12)),
                '22000000-0000-0000-0000-000000000019',
                'L' + RIGHT('00'+CAST(@iL19 AS VARCHAR),2), @rL19, @cL19, 1, 1);
        SET @cL19 = @cL19 + 1;
        IF @cL19 > 4 BEGIN SET @cL19 = 1; SET @rL19 = @rL19 + 1; END
        SET @iL19 = @iL19 + 1;
    END
END;

-- Xe giuong nam 34 (3 cols, 2 floors)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '33000000-0000-0000-0000-000000000034')
BEGIN
    DECLARE @i34 INT = 1, @f34 INT = 1, @r34 INT = 1, @c34 INT = 1, @fc34 INT = 0;
    WHILE @i34 <= 34
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('33340000-0000-0000-0000-', RIGHT('000000000000'+CAST(@i34 AS VARCHAR),12)),
                '33000000-0000-0000-0000-000000000034',
                CASE WHEN @f34=1 THEN 'A' ELSE 'B' END + RIGHT('00'+CAST(@fc34+1 AS VARCHAR),2),
                @r34, @c34, @f34, 2);
        SET @fc34 = @fc34 + 1;
        SET @c34 = @c34 + 1;
        IF @c34 > 3 BEGIN SET @c34 = 1; SET @r34 = @r34 + 1; END
        IF @fc34 >= 17 BEGIN SET @f34 = 2; SET @fc34 = 0; SET @r34 = 1; SET @c34 = 1; END
        SET @i34 = @i34 + 1;
    END
END;

-- Xe giuong phong don 20 (2 cols, 2 floors)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '55000000-0000-0000-0000-000000000020')
BEGIN
    DECLARE @i20 INT = 1, @f20 INT = 1, @r20 INT = 1, @c20 INT = 1, @fc20 INT = 0;
    WHILE @i20 <= 20
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('55200000-0000-0000-0000-', RIGHT('000000000000'+CAST(@i20 AS VARCHAR),12)),
                '55000000-0000-0000-0000-000000000020',
                'CS' + RIGHT('00'+CAST(@fc20+1 AS VARCHAR),2),
                @r20, @c20, @f20, 3);
        SET @fc20 = @fc20 + 1;
        SET @c20 = @c20 + 1;
        IF @c20 > 2 BEGIN SET @c20 = 1; SET @r20 = @r20 + 1; END
        IF @fc20 >= 10 BEGIN SET @f20 = 2; SET @fc20 = 0; SET @r20 = 1; SET @c20 = 1; END
        SET @i20 = @i20 + 1;
    END
END;

-- Xe giuong phong don 24 (2 cols, 2 floors)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '55000000-0000-0000-0000-000000000024')
BEGIN
    DECLARE @i24s INT = 1, @f24s INT = 1, @r24s INT = 1, @c24s INT = 1, @fc24s INT = 0;
    WHILE @i24s <= 24
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('55240000-0000-0000-0000-', RIGHT('000000000000'+CAST(@i24s AS VARCHAR),12)),
                '55000000-0000-0000-0000-000000000024',
                'CS' + RIGHT('00'+CAST(@fc24s+1 AS VARCHAR),2),
                @r24s, @c24s, @f24s, 3);
        SET @fc24s = @fc24s + 1;
        SET @c24s = @c24s + 1;
        IF @c24s > 2 BEGIN SET @c24s = 1; SET @r24s = @r24s + 1; END
        IF @fc24s >= 12 BEGIN SET @f24s = 2; SET @fc24s = 0; SET @r24s = 1; SET @c24s = 1; END
        SET @i24s = @i24s + 1;
    END
END;

-- Xe giuong phong doi 22 (2 cols, 2 floors)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '66000000-0000-0000-0000-000000000022')
BEGIN
    DECLARE @i22d INT = 1, @f22d INT = 1, @r22d INT = 1, @c22d INT = 1, @fc22d INT = 0;
    WHILE @i22d <= 22
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('66220000-0000-0000-0000-', RIGHT('000000000000'+CAST(@i22d AS VARCHAR),12)),
                '66000000-0000-0000-0000-000000000022',
                'CD' + RIGHT('00'+CAST(@fc22d+1 AS VARCHAR),2),
                @r22d, @c22d, @f22d, 4);
        SET @fc22d = @fc22d + 1;
        SET @c22d = @c22d + 1;
        IF @c22d > 2 BEGIN SET @c22d = 1; SET @r22d = @r22d + 1; END
        IF @fc22d >= 11 BEGIN SET @f22d = 2; SET @fc22d = 0; SET @r22d = 1; SET @c22d = 1; END
        SET @i22d = @i22d + 1;
    END
END;

-- Xe giuong phong doi 24 (2 cols, 2 floors)
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '66000000-0000-0000-0000-000000000024')
BEGIN
    DECLARE @i24d INT = 1, @f24d INT = 1, @r24d INT = 1, @c24d INT = 1, @fc24d INT = 0;
    WHILE @i24d <= 24
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (CONCAT('66240000-0000-0000-0000-', RIGHT('000000000000'+CAST(@i24d AS VARCHAR),12)),
                '66000000-0000-0000-0000-000000000024',
                'CD' + RIGHT('00'+CAST(@fc24d+1 AS VARCHAR),2),
                @r24d, @c24d, @f24d, 4);
        SET @fc24d = @fc24d + 1;
        SET @c24d = @c24d + 1;
        IF @c24d > 2 BEGIN SET @c24d = 1; SET @r24d = @r24d + 1; END
        IF @fc24d >= 12 BEGIN SET @f24d = 2; SET @fc24d = 0; SET @r24d = 1; SET @c24d = 1; END
        SET @i24d = @i24d + 1;
    END
END;

PRINT 'Done!';
SELECT Name, SeatCount, (SELECT COUNT(*) FROM SeatTemplates st WHERE st.BusTypeId = bt.Id) as Templates
FROM BusTypes bt ORDER BY Name;
USE [WebBanVeXeDB];
GO

-- 1. Xóa dữ liệu cũ để tránh trùng lặp khi chạy lại script
DELETE FROM [Payments];
DELETE FROM [Invoices];
DELETE FROM [BookingDetails];
DELETE FROM [Bookings];
DELETE FROM [Seats];
DELETE FROM [Trips];
DELETE FROM [RouteStops];
DELETE FROM [StopPoints];
DELETE FROM [Routes];
-- Không xóa Users, BusTypes, Buses vì đã có trong schema.sql (EF Migration Seed)

-- 2. Thêm các Điểm dừng (StopPoints)
DECLARE @StopHN_BXMydinh UNIQUEIDENTIFIER = NEWID();
DECLARE @StopHP_BXNiemsat UNIQUEIDENTIFIER = NEWID();
DECLARE @StopSG_BXMientay UNIQUEIDENTIFIER = NEWID();
DECLARE @StopDL_BXDalat UNIQUEIDENTIFIER = NEWID();

INSERT INTO [StopPoints] ([Id], [Name], [Address], [IsPickup], [IsDropoff], [Badge])
VALUES 
(@StopHN_BXMydinh, N'Bến xe Mỹ Đình', N'Phạm Hùng, Hà Nội', 1, 1, N'Hà Nội'),
(@StopHP_BXNiemsat, N'Bến xe Niệm Nghĩa', N'Trần Nguyên Hãn, Hải Phòng', 1, 1, N'Hải Phòng'),
(@StopSG_BXMientay, N'Bến xe Miền Tây', N'Kinh Dương Vương, TP.HCM', 1, 1, N'Sài Gòn'),
(@StopDL_BXDalat, N'Bến xe Liên tỉnh Đà Lạt', N'Tô Hiến Thành, Đà Lạt', 1, 1, N'Đà Lạt');

-- 3. Thêm Tuyến đường (Routes) mẫu
DECLARE @Route1 UNIQUEIDENTIFIER = '11111111-2222-3333-4444-555555555555'; -- Hà Nội - Hải Phòng
DECLARE @Route2 UNIQUEIDENTIFIER = '22222222-3333-4444-5555-666666666666'; -- Sài Gòn - Đà Lạt

INSERT INTO [Routes] ([Id], [Origin], [Destination], [Points], [DistanceKm], [IsActive], [CreatedAt])
VALUES 
(@Route1, N'Hà Nội', N'Hải Phòng', N'Gia Lâm, Hải Dương', 120, 1, GETUTCDATE()),
(@Route2, N'Sài Gòn', N'Đà Lạt', N'Bảo Lộc, Di Linh', 310, 1, GETUTCDATE());

-- 4. Thêm Chặng dừng (RouteStops)
INSERT INTO [RouteStops] ([Id], [RouteId], [StopPointId], [OffsetMinutes], [DistanceFromOriginKm], [OrderIndex])
VALUES 
(NEWID(), @Route1, @StopHN_BXMydinh, 0, 0, 0),
(NEWID(), @Route1, @StopHP_BXNiemsat, 120, 120, 1),
(NEWID(), @Route2, @StopSG_BXMientay, 0, 0, 0),
(NEWID(), @Route2, @StopDL_BXDalat, 420, 310, 1);

-- 5. Thêm Chuyến đi (Trips)
DECLARE @BusSleeper UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555555'; -- Phương Trang Sleeper
DECLARE @BusLimousine UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666666'; -- Thành Bưởi Limousine

DECLARE @Trip1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Trip2 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [Trips] ([Id], [RouteId], [BusId], [DepartureTime], [ArrivalTime], [Price], [Status], [CreatedAt])
VALUES 
(@Trip1, @Route1, @BusSleeper, DATEADD(hour, 2, GETUTCDATE()), DATEADD(hour, 4, GETUTCDATE()), 150000, N'Active', GETUTCDATE()),
(@Trip2, @Route2, @BusLimousine, DATEADD(hour, 10, GETUTCDATE()), DATEADD(hour, 17, GETUTCDATE()), 350000, N'Active', GETUTCDATE());

-- 6. Sinh tự động ghế (Seats) cho Trip1 (Sleeper 44 chỗ)
-- Để đơn giản, seed 5 ghế mẫu cho mỗi trip
INSERT INTO [Seats] ([Id], [TripId], [SeatNumber], [RowNumber], [ColumnNumber], [Floor], [Type], [Status], [LockedByUserId])
VALUES 
(NEWID(), @Trip1, 'A01', 1, 1, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'A02', 1, 2, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'A03', 1, 3, 1, 1, 0, NULL),
(NEWID(), @Trip2, 'L01', 1, 1, 1, 1, 0, NULL),
(NEWID(), @Trip2, 'L02', 1, 2, 1, 1, 0, NULL);

-- 7. Thêm một số người dùng mẫu
INSERT INTO [Users] ([Id], [UserName], [Email], [PasswordHash], [FullName], [PhoneNumber], [Role], [CreatedAt], [IsActive])
VALUES 
(NEWID(), 'customer1', 'customer1@gmail.com', '$2a$11$.N14FFcQMoCcN3OZvl3DkOMgSok1ocYfR2qyWMxVmlpuqUxxl20la', N'Nguyễn Văn Khách', '0912345678', 'Customer', GETDATE(), 1),
(NEWID(), 'staff1', 'staff1@vexesystem.com', '$2a$11$.N14FFcQMoCcN3OZvl3DkOMgSok1ocYfR2qyWMxVmlpuqUxxl20la', N'Trần Thị Nhân Viên', '0987654321', 'Staff', GETDATE(), 1);

PRINT 'Seed Data executed successfully!';
GO
