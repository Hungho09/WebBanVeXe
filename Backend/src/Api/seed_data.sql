USE [WebBanVeXeDB];
GO

-- 1. Clean existing mock data if any (optional, but good for fresh start)
DELETE FROM [Seats];
DELETE FROM [Trips];
DELETE FROM [Buses];
DELETE FROM [Routes];

-- Hardcoded GUIDs for easy reference
DECLARE @RouteId UNIQUEIDENTIFIER = '11111111-2222-3333-4444-555555555555';
DECLARE @BusId UNIQUEIDENTIFIER = '22222222-3333-4444-5555-666666666666';
DECLARE @TripId UNIQUEIDENTIFIER = '33333333-4444-5555-6666-777777777777';

-- 2. Insert Route (Sài Gòn - Đà Lạt)
INSERT INTO [Routes] ([Id], [Origin], [Destination], [Points], [DistanceKm], [IsActive], [CreatedAt])
VALUES (@RouteId, N'Sài Gòn', N'Đà Lạt', N'Biên Hòa, Bảo Lộc', 300.00, 1, GETUTCDATE());

-- 3. Insert Bus (Xe Giường Nằm 36 chỗ)
-- Assuming BusType 0 is Normal/Sleeper based on Enum
INSERT INTO [Buses] ([Id], [PlateNumber], [SeatCapacity], [BusType], [IsActive])
VALUES (@BusId, '51B-999.99', 36, 0, 1);

-- 4. Insert Trip
INSERT INTO [Trips] ([Id], [RouteId], [BusId], [DepartureTime], [ArrivalTime], [Price], [Status], [CreatedAt])
VALUES (@TripId, @RouteId, @BusId, DATEADD(hour, 10, GETUTCDATE()), DATEADD(hour, 18, GETUTCDATE()), 250000.00, 'Active', GETUTCDATE());

-- 5. Insert 36 Seats for the Trip based on Mock Layout (2 Floors, 3 Columns, 6 Rows)
-- Floor 1
INSERT INTO [Seats] ([Id], [TripId], [SeatNumber], [Status], [Type], [Floor], [RowNumber], [ColumnNumber]) VALUES 
(NEWID(), @TripId, 'A1D', 0, 0, 1, 1, 1), (NEWID(), @TripId, 'B1D', 0, 0, 1, 1, 2), (NEWID(), @TripId, 'C1D', 0, 0, 1, 1, 3),
(NEWID(), @TripId, 'A2D', 0, 0, 1, 2, 1), (NEWID(), @TripId, 'B2D', 0, 0, 1, 2, 2), (NEWID(), @TripId, 'C2D', 0, 0, 1, 2, 3),
(NEWID(), @TripId, 'A3D', 0, 0, 1, 3, 1), (NEWID(), @TripId, 'B3D', 0, 0, 1, 3, 2), (NEWID(), @TripId, 'C3D', 0, 0, 1, 3, 3),
(NEWID(), @TripId, 'A4D', 0, 0, 1, 4, 1), (NEWID(), @TripId, 'B4D', 0, 0, 1, 4, 2), (NEWID(), @TripId, 'C4D', 0, 0, 1, 4, 3),
(NEWID(), @TripId, 'A5D', 0, 0, 1, 5, 1), (NEWID(), @TripId, 'B5D', 0, 0, 1, 5, 2), (NEWID(), @TripId, 'C5D', 0, 0, 1, 5, 3),
(NEWID(), @TripId, 'A6D', 0, 0, 1, 6, 1), (NEWID(), @TripId, 'B6D', 0, 0, 1, 6, 2), (NEWID(), @TripId, 'C6D', 0, 0, 1, 6, 3);

-- Floor 2
INSERT INTO [Seats] ([Id], [TripId], [SeatNumber], [Status], [Type], [Floor], [RowNumber], [ColumnNumber]) VALUES 
(NEWID(), @TripId, 'A1T', 0, 0, 2, 1, 1), (NEWID(), @TripId, 'B1T', 0, 0, 2, 1, 2), (NEWID(), @TripId, 'C1T', 0, 0, 2, 1, 3),
(NEWID(), @TripId, 'A2T', 0, 0, 2, 2, 1), (NEWID(), @TripId, 'B2T', 0, 0, 2, 2, 2), (NEWID(), @TripId, 'C2T', 0, 0, 2, 2, 3),
(NEWID(), @TripId, 'A3T', 0, 0, 2, 3, 1), (NEWID(), @TripId, 'B3T', 0, 0, 2, 3, 2), (NEWID(), @TripId, 'C3T', 0, 0, 2, 3, 3),
(NEWID(), @TripId, 'A4T', 0, 0, 2, 4, 1), (NEWID(), @TripId, 'B4T', 0, 0, 2, 4, 2), (NEWID(), @TripId, 'C4T', 0, 0, 2, 4, 3),
(NEWID(), @TripId, 'A5T', 0, 0, 2, 5, 1), (NEWID(), @TripId, 'B5T', 0, 0, 2, 5, 2), (NEWID(), @TripId, 'C5T', 0, 0, 2, 5, 3),
(NEWID(), @TripId, 'A6T', 0, 0, 2, 6, 1), (NEWID(), @TripId, 'B6T', 0, 0, 2, 6, 2), (NEWID(), @TripId, 'C6T', 0, 0, 2, 6, 3);

PRINT 'Thêm dữ liệu mẫu thành công! TripID để test: 33333333-4444-5555-6666-777777777777';
GO
