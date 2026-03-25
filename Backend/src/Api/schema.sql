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
CREATE TABLE [Buses] (
    [Id] uniqueidentifier NOT NULL,
    [PlateNumber] nvarchar(50) NOT NULL,
    [BusType] int NOT NULL,
    [SeatCapacity] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Buses] PRIMARY KEY ([Id])
);

CREATE TABLE [Routes] (
    [Id] uniqueidentifier NOT NULL,
    [Origin] nvarchar(200) NOT NULL,
    [Points] nvarchar(200) NOT NULL,
    [Destination] nvarchar(200) NOT NULL,
    [DistanceKm] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Routes] PRIMARY KEY ([Id])
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

CREATE TABLE [Trips] (
    [Id] uniqueidentifier NOT NULL,
    [RouteId] uniqueidentifier NOT NULL,
    [BusId] uniqueidentifier NOT NULL,
    [DepartureTime] datetime2 NOT NULL,
    [ArrivalTime] datetime2 NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Trips] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Trips_Buses_BusId] FOREIGN KEY ([BusId]) REFERENCES [Buses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Trips_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Notifications] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [IsSent] bit NOT NULL,
    [SentAt] datetime2 NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Bookings] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [TripId] uniqueidentifier NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [BookingStatus] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Bookings_Trips_TripId] FOREIGN KEY ([TripId]) REFERENCES [Trips] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Bookings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Seats] (
    [Id] uniqueidentifier NOT NULL,
    [TripId] uniqueidentifier NOT NULL,
    [SeatNumber] nvarchar(10) NOT NULL,
    [RowNumber] int NOT NULL,
    [ColumnNumber] int NOT NULL,
    [Floor] int NOT NULL,
    [Status] int NOT NULL,
    [LockExpirationTime] datetime2 NULL,
    [LockedByUserId] uniqueidentifier NULL,
    CONSTRAINT [PK_Seats] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Seats_Trips_TripId] FOREIGN KEY ([TripId]) REFERENCES [Trips] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Invoices] (
    [Id] uniqueidentifier NOT NULL,
    [BookingId] uniqueidentifier NOT NULL,
    [InvoiceNumber] nvarchar(50) NOT NULL,
    [IssuedDate] datetime2 NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Invoices_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Payments] (
    [Id] uniqueidentifier NOT NULL,
    [BookingId] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [PaymentStatus] int NOT NULL,
    [PaymentMethod] nvarchar(50) NOT NULL,
    [TransactionCode] nvarchar(100) NOT NULL,
    [PaidAt] datetime2 NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [BookingDetails] (
    [Id] uniqueidentifier NOT NULL,
    [BookingId] uniqueidentifier NOT NULL,
    [SeatId] uniqueidentifier NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_BookingDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BookingDetails_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]),
    CONSTRAINT [FK_BookingDetails_Seats_SeatId] FOREIGN KEY ([SeatId]) REFERENCES [Seats] ([Id])
);

CREATE INDEX [IX_BookingDetails_BookingId] ON [BookingDetails] ([BookingId]);

CREATE INDEX [IX_BookingDetails_SeatId] ON [BookingDetails] ([SeatId]);

CREATE INDEX [IX_Bookings_TripId] ON [Bookings] ([TripId]);

CREATE INDEX [IX_Bookings_UserId] ON [Bookings] ([UserId]);

CREATE INDEX [IX_Invoices_BookingId] ON [Invoices] ([BookingId]);

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);

CREATE INDEX [IX_Payments_BookingId] ON [Payments] ([BookingId]);

CREATE INDEX [IX_Seats_TripId] ON [Seats] ([TripId]);

CREATE INDEX [IX_Trips_BusId] ON [Trips] ([BusId]);

CREATE INDEX [IX_Trips_RouteId] ON [Trips] ([RouteId]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260306022700_InitialCreate', N'9.0.0');

ALTER TABLE [BookingDetails] DROP CONSTRAINT [FK_BookingDetails_Bookings_BookingId];

ALTER TABLE [BookingDetails] DROP CONSTRAINT [FK_BookingDetails_Seats_SeatId];

ALTER TABLE [Bookings] DROP CONSTRAINT [FK_Bookings_Trips_TripId];

ALTER TABLE [Bookings] DROP CONSTRAINT [FK_Bookings_Users_UserId];

ALTER TABLE [Invoices] DROP CONSTRAINT [FK_Invoices_Bookings_BookingId];

ALTER TABLE [Notifications] DROP CONSTRAINT [FK_Notifications_Users_UserId];

ALTER TABLE [Payments] DROP CONSTRAINT [FK_Payments_Bookings_BookingId];

ALTER TABLE [Seats] DROP CONSTRAINT [FK_Seats_Trips_TripId];

ALTER TABLE [Trips] DROP CONSTRAINT [FK_Trips_Buses_BusId];

ALTER TABLE [Trips] DROP CONSTRAINT [FK_Trips_Routes_RouteId];

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'FullName', N'IsActive', N'PasswordHash', N'PhoneNumber', N'Role', N'UserName') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [CreatedAt], [Email], [FullName], [IsActive], [PasswordHash], [PhoneNumber], [Role], [UserName])
VALUES ('11111111-1111-1111-1111-111111111111', '2026-01-01T00:00:00.0000000Z', N'admin@vexesystem.com', N'System Administrator', CAST(1 AS bit), N'$2a$11$0nK18Qc7D8N94B3U3P6S/OGfN9f4v.T2H6zH/r4O/C5v.Q/b4XvG6', N'0123456789', N'Admin', N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Email', N'FullName', N'IsActive', N'PasswordHash', N'PhoneNumber', N'Role', N'UserName') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;

ALTER TABLE [BookingDetails] ADD CONSTRAINT [FK_BookingDetails_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [BookingDetails] ADD CONSTRAINT [FK_BookingDetails_Seats_SeatId] FOREIGN KEY ([SeatId]) REFERENCES [Seats] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Bookings] ADD CONSTRAINT [FK_Bookings_Trips_TripId] FOREIGN KEY ([TripId]) REFERENCES [Trips] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Bookings] ADD CONSTRAINT [FK_Bookings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Invoices] ADD CONSTRAINT [FK_Invoices_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Seats] ADD CONSTRAINT [FK_Seats_Trips_TripId] FOREIGN KEY ([TripId]) REFERENCES [Trips] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Trips] ADD CONSTRAINT [FK_Trips_Buses_BusId] FOREIGN KEY ([BusId]) REFERENCES [Buses] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [Trips] ADD CONSTRAINT [FK_Trips_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260306025511_FixCascadeAndSeed', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260313120844_AddBusEntity_BVX46', N'9.0.0');

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Routes]') AND [c].[name] = N'Points');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Routes] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Routes] ALTER COLUMN [Points] nvarchar(500) NOT NULL;

ALTER TABLE [Routes] ADD [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit);

ALTER TABLE [Routes] ADD [UpdatedAt] datetime2 NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260318115618_UpdateSchema', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260319161807_AddUserIsActive', N'9.0.0');

ALTER TABLE [Seats] ADD [Type] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260320164834_BVX71_AddSeatTypeAndMaintenance', N'9.0.0');

CREATE TABLE [SeatTemplates] (
    [Id] uniqueidentifier NOT NULL,
    [BusType] int NOT NULL,
    [SeatNumber] nvarchar(10) NOT NULL,
    [RowNumber] int NOT NULL,
    [ColumnNumber] int NOT NULL,
    [Floor] int NOT NULL,
    [Type] int NOT NULL,
    CONSTRAINT [PK_SeatTemplates] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260320170417_BVX73_AddSeatTemplate', N'9.0.0');

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Trips]') AND [c].[name] = N'Status');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Trips] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Trips] ALTER COLUMN [Status] nvarchar(50) NOT NULL;

ALTER TABLE [Trips] ADD [UpdatedAt] datetime2 NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260323102456_UpdateModels', N'9.0.0');

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BusType', N'ColumnNumber', N'Floor', N'RowNumber', N'SeatNumber', N'Type') AND [object_id] = OBJECT_ID(N'[SeatTemplates]'))
    SET IDENTITY_INSERT [SeatTemplates] ON;
INSERT INTO [SeatTemplates] ([Id], [BusType], [ColumnNumber], [Floor], [RowNumber], [SeatNumber], [Type])
VALUES ('10000000-0000-0000-0000-000000000001', 0, 1, 1, 1, N'A01', 1),
('10000000-0000-0000-0000-000000000002', 0, 2, 1, 1, N'A02', 1),
('10000000-0000-0000-0000-000000000003', 0, 3, 1, 1, N'A03', 1),
('10000000-0000-0000-0000-000000000004', 0, 1, 1, 2, N'A04', 0),
('10000000-0000-0000-0000-000000000005', 0, 2, 1, 2, N'A05', 0),
('10000000-0000-0000-0000-000000000006', 0, 3, 1, 2, N'A06', 0),
('10000000-0000-0000-0000-000000000007', 0, 1, 1, 3, N'A07', 0),
('10000000-0000-0000-0000-000000000008', 0, 2, 1, 3, N'A08', 0),
('10000000-0000-0000-0000-000000000009', 0, 3, 1, 3, N'A09', 0),
('10000000-0000-0000-0000-000000000010', 0, 1, 1, 4, N'A10', 0),
('10000000-0000-0000-0000-000000000011', 0, 2, 1, 4, N'A11', 0),
('10000000-0000-0000-0000-000000000012', 0, 3, 1, 4, N'A12', 0),
('10000000-0000-0000-0000-000000000013', 0, 1, 1, 5, N'A13', 0),
('10000000-0000-0000-0000-000000000014', 0, 2, 1, 5, N'A14', 0),
('10000000-0000-0000-0000-000000000015', 0, 3, 1, 5, N'A15', 0),
('10000000-0000-0000-0000-000000000016', 0, 1, 1, 6, N'A16', 0),
('10000000-0000-0000-0000-000000000017', 0, 2, 1, 6, N'A17', 0),
('10000000-0000-0000-0000-000000000018', 0, 3, 1, 6, N'A18', 0),
('20000000-0000-0000-0000-000000000019', 0, 1, 2, 1, N'B01', 1),
('20000000-0000-0000-0000-000000000020', 0, 2, 2, 1, N'B02', 1),
('20000000-0000-0000-0000-000000000021', 0, 3, 2, 1, N'B03', 1),
('20000000-0000-0000-0000-000000000022', 0, 1, 2, 2, N'B04', 0),
('20000000-0000-0000-0000-000000000023', 0, 2, 2, 2, N'B05', 0),
('20000000-0000-0000-0000-000000000024', 0, 3, 2, 2, N'B06', 0),
('20000000-0000-0000-0000-000000000025', 0, 1, 2, 3, N'B07', 0),
('20000000-0000-0000-0000-000000000026', 0, 2, 2, 3, N'B08', 0),
('20000000-0000-0000-0000-000000000027', 0, 3, 2, 3, N'B09', 0),
('20000000-0000-0000-0000-000000000028', 0, 1, 2, 4, N'B10', 0),
('20000000-0000-0000-0000-000000000029', 0, 2, 2, 4, N'B11', 0),
('20000000-0000-0000-0000-000000000030', 0, 3, 2, 4, N'B12', 0),
('20000000-0000-0000-0000-000000000031', 0, 1, 2, 5, N'B13', 0),
('20000000-0000-0000-0000-000000000032', 0, 2, 2, 5, N'B14', 0),
('20000000-0000-0000-0000-000000000033', 0, 3, 2, 5, N'B15', 0),
('20000000-0000-0000-0000-000000000034', 0, 1, 2, 6, N'B16', 0),
('20000000-0000-0000-0000-000000000035', 0, 2, 2, 6, N'B17', 0),
('20000000-0000-0000-0000-000000000036', 0, 3, 2, 6, N'B18', 0),
('45000000-0000-0000-0000-000000000001', 1, 1, 1, 1, N'S01', 1),
('45000000-0000-0000-0000-000000000002', 1, 2, 1, 1, N'S02', 1),
('45000000-0000-0000-0000-000000000003', 1, 3, 1, 1, N'S03', 1),
('45000000-0000-0000-0000-000000000004', 1, 4, 1, 1, N'S04', 1),
('45000000-0000-0000-0000-000000000005', 1, 5, 1, 1, N'S05', 1),
('45000000-0000-0000-0000-000000000006', 1, 1, 1, 2, N'S06', 1);
INSERT INTO [SeatTemplates] ([Id], [BusType], [ColumnNumber], [Floor], [RowNumber], [SeatNumber], [Type])
VALUES ('45000000-0000-0000-0000-000000000007', 1, 2, 1, 2, N'S07', 1),
('45000000-0000-0000-0000-000000000008', 1, 3, 1, 2, N'S08', 1),
('45000000-0000-0000-0000-000000000009', 1, 4, 1, 2, N'S09', 1),
('45000000-0000-0000-0000-000000000010', 1, 5, 1, 2, N'S10', 1),
('45000000-0000-0000-0000-000000000011', 1, 1, 1, 3, N'S11', 0),
('45000000-0000-0000-0000-000000000012', 1, 2, 1, 3, N'S12', 0),
('45000000-0000-0000-0000-000000000013', 1, 3, 1, 3, N'S13', 0),
('45000000-0000-0000-0000-000000000014', 1, 4, 1, 3, N'S14', 0),
('45000000-0000-0000-0000-000000000015', 1, 5, 1, 3, N'S15', 0),
('45000000-0000-0000-0000-000000000016', 1, 1, 1, 4, N'S16', 0),
('45000000-0000-0000-0000-000000000017', 1, 2, 1, 4, N'S17', 0),
('45000000-0000-0000-0000-000000000018', 1, 3, 1, 4, N'S18', 0),
('45000000-0000-0000-0000-000000000019', 1, 4, 1, 4, N'S19', 0),
('45000000-0000-0000-0000-000000000020', 1, 5, 1, 4, N'S20', 0),
('45000000-0000-0000-0000-000000000021', 1, 1, 1, 5, N'S21', 0),
('45000000-0000-0000-0000-000000000022', 1, 2, 1, 5, N'S22', 0),
('45000000-0000-0000-0000-000000000023', 1, 3, 1, 5, N'S23', 0),
('45000000-0000-0000-0000-000000000024', 1, 4, 1, 5, N'S24', 0),
('45000000-0000-0000-0000-000000000025', 1, 5, 1, 5, N'S25', 0),
('45000000-0000-0000-0000-000000000026', 1, 1, 1, 6, N'S26', 0),
('45000000-0000-0000-0000-000000000027', 1, 2, 1, 6, N'S27', 0),
('45000000-0000-0000-0000-000000000028', 1, 3, 1, 6, N'S28', 0),
('45000000-0000-0000-0000-000000000029', 1, 4, 1, 6, N'S29', 0),
('45000000-0000-0000-0000-000000000030', 1, 5, 1, 6, N'S30', 0),
('45000000-0000-0000-0000-000000000031', 1, 1, 1, 7, N'S31', 0),
('45000000-0000-0000-0000-000000000032', 1, 2, 1, 7, N'S32', 0),
('45000000-0000-0000-0000-000000000033', 1, 3, 1, 7, N'S33', 0),
('45000000-0000-0000-0000-000000000034', 1, 4, 1, 7, N'S34', 0),
('45000000-0000-0000-0000-000000000035', 1, 5, 1, 7, N'S35', 0),
('45000000-0000-0000-0000-000000000036', 1, 1, 1, 8, N'S36', 0),
('45000000-0000-0000-0000-000000000037', 1, 2, 1, 8, N'S37', 0),
('45000000-0000-0000-0000-000000000038', 1, 3, 1, 8, N'S38', 0),
('45000000-0000-0000-0000-000000000039', 1, 4, 1, 8, N'S39', 0),
('45000000-0000-0000-0000-000000000040', 1, 5, 1, 8, N'S40', 0),
('45000000-0000-0000-0000-000000000041', 1, 1, 1, 9, N'S41', 0),
('45000000-0000-0000-0000-000000000042', 1, 2, 1, 9, N'S42', 0),
('45000000-0000-0000-0000-000000000043', 1, 3, 1, 9, N'S43', 0),
('45000000-0000-0000-0000-000000000044', 1, 4, 1, 9, N'S44', 0),
('45000000-0000-0000-0000-000000000045', 1, 5, 1, 9, N'S45', 0),
('90000000-0000-0000-0000-000000000001', 2, 1, 1, 1, N'L01', 1),
('90000000-0000-0000-0000-000000000002', 2, 2, 1, 1, N'L02', 1),
('90000000-0000-0000-0000-000000000003', 2, 3, 1, 1, N'L03', 1);
INSERT INTO [SeatTemplates] ([Id], [BusType], [ColumnNumber], [Floor], [RowNumber], [SeatNumber], [Type])
VALUES ('90000000-0000-0000-0000-000000000004', 2, 1, 1, 2, N'L04', 1),
('90000000-0000-0000-0000-000000000005', 2, 2, 1, 2, N'L05', 1),
('90000000-0000-0000-0000-000000000006', 2, 3, 1, 2, N'L06', 1),
('90000000-0000-0000-0000-000000000007', 2, 1, 1, 3, N'L07', 1),
('90000000-0000-0000-0000-000000000008', 2, 2, 1, 3, N'L08', 1),
('90000000-0000-0000-0000-000000000009', 2, 3, 1, 3, N'L09', 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BusType', N'ColumnNumber', N'Floor', N'RowNumber', N'SeatNumber', N'Type') AND [object_id] = OBJECT_ID(N'[SeatTemplates]'))
    SET IDENTITY_INSERT [SeatTemplates] OFF;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260324054120_AddBookingModule', N'9.0.0');

COMMIT;
GO

