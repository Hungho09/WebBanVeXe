USE [WebBanVeXeDB];
GO

-- Xóa dữ liệu cũ để tránh conflict
DELETE FROM [Payments];
DELETE FROM [Invoices];
DELETE FROM [BookingDetails];
DELETE FROM [Bookings];
DELETE FROM [Seats];
DELETE FROM [Trips];
DELETE FROM [RouteStops];
DELETE FROM [StopPoints];
DELETE FROM [Routes];
-- Gi lại Users, BusTypes, Buses vì có trong schema.sql

-- 1. Thêm các Điểm dừng (StopPoints)
DECLARE @StopHN_BXMydinh UNIQUEIDENTIFIER = NEWID();
DECLARE @StopHP_BXNiemsat UNIQUEIDENTIFIER = NEWID();
DECLARE @StopSG_BXMientay UNIQUEIDENTIFIER = NEWID();
DECLARE @StopDL_BXDalat UNIQUEIDENTIFIER = NEWID();
DECLARE @StopDN_BXDaNang UNIQUEIDENTIFIER = NEWID();

INSERT INTO [StopPoints] ([Id], [Name], [Address], [IsPickup], [IsDropoff], [Badge])
VALUES 
(@StopHN_BXMydinh, N'Bến xe Mỹ Đình', N'Phạm Hùng, Hà Nội', 1, 1, N'Hà Nội'),
(@StopHP_BXNiemsat, N'Bến xe Niệm Nghĩa', N'Trần Nguyên Hãn, Hải Phòng', 1, 1, N'Hải Phòng'),
(@StopSG_BXMientay, N'Bến xe Miền Tây', N'Kinh Dương Vương, TP.HCM', 1, 1, N'Sài Gòn'),
(@StopDL_BXDalat, N'Bến xe Liên tỉnh Đà Lạt', N'Tô Hiến Thành, Đà Lạt', 1, 1, N'Đà Lạt'),
(@StopDN_BXDaNang, N'Bến xe Đà Nẵng', N'Tô Hiến Thành, Đà Nẵng', 1, 1, N'Đà Nẵng');

-- 2. Thêm Tuyến đường (Routes) mẫu
DECLARE @Route1 UNIQUEIDENTIFIER = '11111111-2222-3333-4444-555555555555'; -- Hà Nội - Hải Phòng
DECLARE @Route2 UNIQUEIDENTIFIER = '22222222-3333-4444-5555-666666666666'; -- Sài Gòn - Đà Lạt
DECLARE @Route3 UNIQUEIDENTIFIER = '33333333-4444-5555-6666-777777777777'; -- Hà Nội - Đà Nẵng

INSERT INTO [Routes] ([Id], [Origin], [Destination], [Points], [DistanceKm], [IsActive], [CreatedAt])
VALUES 
(@Route1, N'Hà Nội', N'Hải Phòng', N'Gia Lâm, Hải Dương', 120, 1, GETUTCDATE()),
(@Route2, N'Sài Gòn', N'Đà Lạt', N'Bảo Lộc, Di Linh', 310, 1, GETUTCDATE()),
(@Route3, N'Hà Nội', N'Đà Nẵng', N'Thanh Hóa, Vinh, Hà Tĩnh', 760, 1, GETUTCDATE());

-- 3. Thêm Chặng dừng (RouteStops)
INSERT INTO [RouteStops] ([Id], [RouteId], [StopPointId], [OffsetMinutes], [DistanceFromOriginKm], [OrderIndex])
VALUES 
(NEWID(), @Route1, @StopHN_BXMydinh, 0, 0, 0),
(NEWID(), @Route1, @StopHP_BXNiemsat, 120, 120, 1),
(NEWID(), @Route2, @StopSG_BXMientay, 0, 0, 0),
(NEWID(), @Route2, @StopDL_BXDalat, 420, 310, 1),
(NEWID(), @Route3, @StopHN_BXMydinh, 0, 0, 0),
(NEWID(), @Route3, @StopDN_BXDaNang, 600, 760, 1);

-- 4. Thêm Chuyến đi (Trips) với thời gian thực tế
DECLARE @BusSleeper UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555555'; -- Phương Trang Sleeper
DECLARE @BusLimousine UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666666'; -- Thành Bưởi Limousine

DECLARE @Trip1 UNIQUEIDENTIFIER = NEWID(); -- Hà Nội - Hải Phòng (sleeper)
DECLARE @Trip2 UNIQUEIDENTIFIER = NEWID(); -- Sài Gòn - Đà Lạt (limousine)
DECLARE @Trip3 UNIQUEIDENTIFIER = NEWID(); -- Hà Nội - Đà Nẵng (sleeper)

INSERT INTO [Trips] ([Id], [RouteId], [BusId], [DepartureTime], [ArrivalTime], [Price], [Status], [CreatedAt])
VALUES 
(@Trip1, @Route1, @BusSleeper, DATEADD(hour, 2, GETUTCDATE()), DATEADD(hour, 4, GETUTCDATE()), 150000, N'Active', GETUTCDATE()),
(@Trip2, @Route2, @BusLimousine, DATEADD(hour, 10, GETUTCDATE()), DATEADD(hour, 17, GETUTCDATE()), 350000, N'Active', GETUTCDATE()),
(@Trip3, @Route3, @BusSleeper, DATEADD(hour, 18, GETUTCDATE()), DATEADD(hour, 30, GETUTCDATE()), 450000, N'Active', GETUTCDATE());

-- 5. Thêm Users mẫu (nếu chưa có)
DECLARE @Customer1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Customer2 UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT 1 FROM [Users] WHERE UserName = 'customer1')
BEGIN
    INSERT INTO [Users] ([Id], [UserName], [Email], [PasswordHash], [FullName], [PhoneNumber], [Role], [CreatedAt], [IsActive])
    VALUES 
    (@Customer1, 'customer1', 'customer1@gmail.com', '$2a$11$0nK18Qc7D8N94B3U3P6S/OGfN9f4v.T2H6zH/r4O/C5v.Q/b4XvG6', N'Nguyễn Văn Khách', '0912345678', 'Customer', GETDATE(), 1),
    (@Customer2, 'customer2', 'customer2@gmail.com', '$2a$11$0nK18Qc7D8N94B3U3P6S/OGfN9f4v.T2H6zH/r4O/C5v.Q/b4XvG6', N'Tran Thi Mai', '0987654321', 'Customer', GETDATE(), 1);
END
ELSE
BEGIN
    SELECT @Customer1 = Id FROM [Users] WHERE UserName = 'customer1';
    SELECT @Customer2 = Id FROM [Users] WHERE UserName = 'customer2';
END

-- 6. Sinh ghế (Seats) cho các Trip
-- Trip1: Hà Nội - Hải Phòng (Sleeper 44 chỗ) - tạo 10 ghế mẫu
INSERT INTO [Seats] ([Id], [TripId], [SeatNumber], [RowNumber], [ColumnNumber], [Floor], [Type], [Status], [LockedByUserId])
VALUES 
(NEWID(), @Trip1, 'A01', 1, 1, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'A02', 1, 2, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'A03', 1, 3, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'A04', 1, 4, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'A05', 1, 5, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'B01', 2, 1, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'B02', 2, 2, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'B03', 2, 3, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'B04', 2, 4, 1, 1, 0, NULL),
(NEWID(), @Trip1, 'B05', 2, 5, 1, 1, 0, NULL);

-- Trip2: Sài Gòn - Đà Lạt (Limousine 9 chỗ) - tạo 9 ghế
INSERT INTO [Seats] ([Id], [TripId], [SeatNumber], [RowNumber], [ColumnNumber], [Floor], [Type], [Status], [LockedByUserId])
VALUES 
(NEWID(), @Trip2, 'L01', 1, 1, 1, 2, 0, NULL),
(NEWID(), @Trip2, 'L02', 1, 2, 1, 2, 0, NULL),
(NEWID(), @Trip2, 'L03', 1, 3, 1, 2, 0, NULL),
(NEWID(), @Trip2, 'L04', 2, 1, 1, 2, 0, NULL),
(NEWID(), @Trip2, 'L05', 2, 2, 1, 2, 0, NULL),
(NEWID(), @Trip2, 'L06', 2, 3, 1, 2, 0, NULL),
(NEWID(), @Trip2, 'L07', 3, 1, 1, 2, 0, NULL),
(NEWID(), @Trip2, 'L08', 3, 2, 1, 2, 0, NULL),
(NEWID(), @Trip2, 'L09', 3, 3, 1, 2, 0, NULL);

-- Trip3: Hà Nội - Đà Nẵng (Sleeper 44 chỗ) - tạo 8 ghế mẫu
INSERT INTO [Seats] ([Id], [TripId], [SeatNumber], [RowNumber], [ColumnNumber], [Floor], [Type], [Status], [LockedByUserId])
VALUES 
(NEWID(), @Trip3, 'C01', 1, 1, 1, 1, 0, NULL),
(NEWID(), @Trip3, 'C02', 1, 2, 1, 1, 0, NULL),
(NEWID(), @Trip3, 'C03', 1, 3, 1, 1, 0, NULL),
(NEWID(), @Trip3, 'C04', 1, 4, 1, 1, 0, NULL),
(NEWID(), @Trip3, 'C05', 2, 1, 1, 1, 0, NULL),
(NEWID(), @Trip3, 'C06', 2, 2, 1, 1, 0, NULL),
(NEWID(), @Trip3, 'C07', 2, 3, 1, 1, 0, NULL),
(NEWID(), @Trip3, 'C08', 2, 4, 1, 1, 0, NULL);

-- 7. Tạo Bookings mẫu với trạng thái khác nhau
DECLARE @Booking1 UNIQUEIDENTIFIER = NEWID(); -- Paid booking
DECLARE @Booking2 UNIQUEIDENTIFIER = NEWID(); -- Pending booking
DECLARE @Booking3 UNIQUEIDENTIFIER = NEWID(); -- Cancelled booking

INSERT INTO [Bookings] ([Id], [UserId], [TripId], [TotalAmount], [BookingStatus], [CreatedAt], [PickupPointId], [DropoffPointId])
VALUES 
(@Booking1, @Customer1, @Trip1, 300000, 1, DATEADD(hour, -2, GETUTCDATE()), @StopHN_BXMydinh, @StopHP_BXNiemsat), -- 1 = Paid
(@Booking2, @Customer2, @Trip2, 700000, 0, DATEADD(hour, -1, GETUTCDATE()), @StopSG_BXMientay, @StopDL_BXDalat), -- 0 = Pending
(@Booking3, @Customer1, @Trip3, 900000, 3, DATEADD(hour, -3, GETUTCDATE()), @StopHN_BXMydinh, @StopDN_BXDaNang); -- 3 = Cancelled

-- 8. Thêm BookingDetails cho từng booking
-- Booking1: 2 ghế trên trip Hà Nội - Hải Phòng
INSERT INTO [BookingDetails] ([Id], [BookingId], [SeatId], [Price])
VALUES 
(NEWID(), @Booking1, (SELECT TOP 1 Id FROM Seats WHERE TripId = @Trip1 AND SeatNumber = 'A01'), 150000),
(NEWID(), @Booking1, (SELECT TOP 1 Id FROM Seats WHERE TripId = @Trip1 AND SeatNumber = 'A02'), 150000);

-- Booking2: 2 ghế trên trip Sài Gòn - Đà Lạt
INSERT INTO [BookingDetails] ([Id], [BookingId], [SeatId], [Price])
VALUES 
(NEWID(), @Booking2, (SELECT TOP 1 Id FROM Seats WHERE TripId = @Trip2 AND SeatNumber = 'L01'), 350000),
(NEWID(), @Booking2, (SELECT TOP 1 Id FROM Seats WHERE TripId = @Trip2 AND SeatNumber = 'L02'), 350000);

-- Booking3: 2 ghế trên trip Hà Nội - Đà Nẵng (đã hủy)
INSERT INTO [BookingDetails] ([Id], [BookingId], [SeatId], [Price])
VALUES 
(NEWID(), @Booking3, (SELECT TOP 1 Id FROM Seats WHERE TripId = @Trip3 AND SeatNumber = 'C01'), 450000),
(NEWID(), @Booking3, (SELECT TOP 1 Id FROM Seats WHERE TripId = @Trip3 AND SeatNumber = 'C02'), 450000);

-- 9. Cập nhật trạng thái ghế (booked cho booking đã thanh toán, available cho booking đã hủy)
UPDATE [Seats] SET Status = 2 WHERE TripId = @Trip1 AND SeatNumber IN ('A01', 'A02'); -- Booked
UPDATE [Seats] SET Status = 2 WHERE TripId = @Trip2 AND SeatNumber IN ('L01', 'L02'); -- Booked
-- Ghế của booking3 đã hủy nên vẫn available (status = 0)

-- 10. Tạo Invoices cho bookings đã thanh toán
DECLARE @Invoice1 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [Invoices] ([Id], [InvoiceNumber], [BookingId], [CustomerName], [CustomerEmail], [TotalAmount], [CreatedAt], [Status])
VALUES 
(@Invoice1, 'INV20260326001', @Booking1, 'Nguyễn Văn Khách', 'customer1@gmail.com', 300000, DATEADD(hour, -2, GETUTCDATE()), 1); -- 1 = Paid

-- 11. Tạo Payments cho bookings đã thanh toán
INSERT INTO [Payments] ([Id], [BookingId], [Amount], [PaymentMethod], [PaymentStatus], [TransactionCode], [PaidAt])
VALUES 
(NEWID(), @Booking1, 300000, 'CreditCard', 2, 'TXN123456789', DATEADD(hour, -2, GETUTCDATE())); -- 2 = Completed

PRINT 'Sample data for testing booking, payment, and invoice flow created successfully!';
PRINT 'Login credentials:';
PRINT 'Admin: admin / Admin@123';
PRINT 'Customer1: customer1 / Admin@123';
PRINT 'Customer2: customer2 / Admin@123';
GO
