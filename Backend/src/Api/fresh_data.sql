USE [WebBanVeXeDB];
GO

-- 1. Xóa dữ liệu cũ theo đúng thứ tự ràng buộc
DELETE FROM [Payments];
DELETE FROM [Invoices];
DELETE FROM [BookingDetails];
DELETE FROM [Bookings];
DELETE FROM [Seats];
DELETE FROM [Trips];
DELETE FROM [RouteStops];
DELETE FROM [StopPoints];
DELETE FROM [Routes];
DELETE FROM [Buses];
DELETE FROM [BusTypes];
DELETE FROM [Users];
DELETE FROM [CmsConfigs];
GO

-- 2. Nạp dữ liệu cơ sở
-- BusTypes
DECLARE @BusTypeGiuongUnique UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @BusTypeLimoUnique UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

INSERT INTO [BusTypes] ([Id], [Name], [SeatCount], [Description])
VALUES 
(@BusTypeGiuongUnique, N'Giường Nằm Standard', 44, N'Xe giường nằm tiêu chuẩn 44 chỗ'),
(@BusTypeLimoUnique, N'Limousine VIP', 9, N'Xe Limousine hạng thương gia 9 chỗ');

-- Admin User
INSERT INTO [Users] ([Id], [UserName], [Email], [PasswordHash], [FullName], [PhoneNumber], [Role], [CreatedAt], [IsActive])
VALUES 
('11111111-1111-1111-1111-111111111111', N'admin', N'admin@vexesystem.com', N'$2a$11$0nK18Qc7D8N94B3U3P6S/OGfN9f4v.T2H6zH/r4O/C5v.Q/b4XvG6', N'Quản trị viên', N'0123456789', N'Admin', GETUTCDATE(), 1);

-- Customer User
INSERT INTO [Users] ([Id], [UserName], [Email], [PasswordHash], [FullName], [PhoneNumber], [Role], [CreatedAt], [IsActive])
VALUES 
(NEWID(), N'khachhang1', N'khach1@gmail.com', N'$2a$11$0nK18Qc7D8N94B3U3P6S/OGfN9f4v.T2H6zH/r4O/C5v.Q/b4XvG6', N'Nguyễn Văn Khách', N'0988888888', N'Customer', GETUTCDATE(), 1);

-- Buses
DECLARE @Bus1 UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555555';
DECLARE @Bus2 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666666';

INSERT INTO [Buses] ([Id], [PlateNumber], [CompanyName], [ImageUrl], [BusTypeId], [SeatCount], [Status])
VALUES 
(@Bus1, N'51B-001.23', N'Xe Khách Thành Công', NULL, @BusTypeGiuongUnique, 44, 2), -- 2 = Available
(@Bus2, N'51B-004.56', N'Hải Vân Express', NULL, @BusTypeLimoUnique, 9, 2);

-- Routes
DECLARE @Route1 UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @Route2 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

INSERT INTO [Routes] ([Id], [Origin], [Destination], [Points], [DistanceKm], [IsActive], [CreatedAt])
VALUES 
(@Route1, N'TP. Hồ Chí Minh', N'Vũng Tàu', N'Long Thành, Bà Rịa', 100, 1, GETUTCDATE()),
(@Route2, N'Hà Nội', N'Sapa', N'Lào Cai', 320, 1, GETUTCDATE());

-- StopPoints
DECLARE @StopSG UNIQUEIDENTIFIER = NEWID();
DECLARE @StopVT UNIQUEIDENTIFIER = NEWID();
DECLARE @StopHN UNIQUEIDENTIFIER = NEWID();
DECLARE @StopSP UNIQUEIDENTIFIER = NEWID();

INSERT INTO [StopPoints] ([Id], [Name], [Address], [IsPickup], [IsDropoff], [Badge])
VALUES 
(@StopSG, N'Bến xe Miền Đông', N'Đinh Bộ Lĩnh, Bình Thạnh, TP.HCM', 1, 1, N'TP.HCM'),
(@StopVT, N'Bến xe Vũng Tàu', N'Nam Kỳ Khởi Nghĩa, Vũng Tàu', 1, 1, N'Vũng Tàu'),
(@StopHN, N'Bến xe Mỹ Đình', N'Từ Liêm, Hà Nội', 1, 1, N'Hà Nội'),
(@StopSP, N'Bến xe Sapa', N'Thị trấn Sapa', 1, 1, N'Sapa');

-- RouteStops
INSERT INTO [RouteStops] ([Id], [RouteId], [StopPointId], [OffsetMinutes], [DistanceFromOriginKm], [OrderIndex])
VALUES 
(NEWID(), @Route1, @StopSG, 0, 0, 0),
(NEWID(), @Route1, @StopVT, 120, 100, 1),
(NEWID(), @Route2, @StopHN, 0, 0, 0),
(NEWID(), @Route2, @StopSP, 360, 320, 1);

-- Trips
DECLARE @Trip1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Trip2 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [Trips] ([Id], [RouteId], [BusId], [DepartureTime], [ArrivalTime], [Price], [Status], [CreatedAt])
VALUES 
(@Trip1, @Route1, @Bus1, DATEADD(hour, 5, GETUTCDATE()), DATEADD(hour, 7, GETUTCDATE()), 120000, N'Active', GETUTCDATE()),
(@Trip2, @Route2, @Bus2, DATEADD(hour, 8, GETUTCDATE()), DATEADD(hour, 14, GETUTCDATE()), 350000, N'Active', GETUTCDATE());

-- Seats for Trip 1 (Mẫu vài cái)
INSERT INTO [Seats] ([Id], [TripId], [SeatNumber], [RowNumber], [ColumnNumber], [Floor], [Type], [Status])
VALUES 
(NEWID(), @Trip1, 'A01', 1, 1, 1, 1, 0),
(NEWID(), @Trip1, 'A02', 1, 2, 1, 1, 0),
(NEWID(), @Trip1, 'A03', 1, 3, 1, 1, 0);

-- CMS Config
INSERT INTO [CmsConfigs] ([ConfigKey], [ContentJson], [UpdatedAt])
VALUES ('homepage_v1', '{}', GETUTCDATE());

PRINT 'Fresh Data setup successful!';
GO
