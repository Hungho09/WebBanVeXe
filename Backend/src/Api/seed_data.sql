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
