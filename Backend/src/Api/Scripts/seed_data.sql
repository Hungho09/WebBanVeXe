USE [WebBanVeXeDB];
GO

-- 1. Ensure Admin User exists
IF NOT EXISTS (SELECT 1 FROM Users WHERE UserName = 'admin')
BEGIN
    INSERT INTO [Users] ([Id], [CreatedAt], [Email], [FullName], [IsActive], [PasswordHash], [PhoneNumber], [Role], [UserName])
    VALUES ('11111111-1111-1111-1111-111111111111', GETUTCDATE(), N'admin@vexesystem.com', N'System Administrator', 1, N'$2a$11$.N14FFcQMoCcN3OZvl3DkOMgSok1ocYfR2qyWMxVmlpuqUxxl20la', N'0123456789', N'Admin', N'admin');
END

-- 2. Add sample StopPoints
IF NOT EXISTS (SELECT 1 FROM StopPoints WHERE Name = N'Bến xe Mỹ Đình')
BEGIN
    INSERT INTO [StopPoints] ([Id], [Name], [Address], [IsPickup], [IsDropoff], [ProvinceName], [IsDefault])
    VALUES 
    (NEWID(), N'Bến xe Mỹ Đình', N'Phạm Hùng, Hà Nội', 1, 1, N'Hà Nội', 1),
    (NEWID(), N'Bến xe Niệm Nghĩa', N'Trần Nguyên Hãn, Hải Phòng', 1, 1, N'Hải Phòng', 1),
    (NEWID(), N'Bến xe Miền Tây', N'Kinh Dương Vương, TP.HCM', 1, 1, N'Sài Gòn', 1);
END

-- 3. Add sample Routes
IF NOT EXISTS (SELECT 1 FROM Routes WHERE Origin = N'Hà Nội')
BEGIN
    INSERT INTO [Routes] ([Id], [Origin], [Destination], [Points], [DistanceKm], [IsActive], [CreatedAt])
    VALUES 
    (NEWID(), N'Hà Nội', N'Hải Phòng', N'Gia Lâm, Hải Dương', 120, 1, GETUTCDATE());
END

-- 4. Add sample Buses
IF NOT EXISTS (SELECT 1 FROM Buses WHERE PlateNumber = '51B-123.45')
BEGIN
    DECLARE @BusTypeSleeper UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM BusTypes WHERE Name LIKE N'%Giường%');
    IF @BusTypeSleeper IS NOT NULL
    BEGIN
        INSERT INTO [Buses] ([Id], [BusTypeId], [CompanyName], [ImageUrl], [PlateNumber], [SeatCount], [Status])
        VALUES (NEWID(), @BusTypeSleeper, N'Phương Trang (FUTA)', NULL, N'51B-123.45', 44, 1);
    END
END

PRINT 'Seed Data (Users, Routes, Stops, Buses) loaded successfully!';
GO
