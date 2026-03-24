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
VALUES (N'20260319161807_AddUserIsActive', N'9.0.0');

COMMIT;
GO

