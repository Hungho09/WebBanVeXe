USE [WebBanVeXeDB];
GO

SET NOCOUNT ON;

-- 1. Insert Core BusTypes
IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '11000000-0000-0000-0000-000000000016')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('11000000-0000-0000-0000-000000000016', N'Xe ghế ngồi', 16, 'Standard Normal Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '11000000-0000-0000-0000-000000000029')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('11000000-0000-0000-0000-000000000029', N'Xe ghế ngồi', 29, 'Standard Normal Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '22000000-0000-0000-0000-000000000011')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('22000000-0000-0000-0000-000000000011', N'Xe Limousine', 11, 'VIP Limousine Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '33000000-0000-0000-0000-000000000034')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('33000000-0000-0000-0000-000000000034', N'Xe giường nằm', 34, 'Standard Sleeper');

-- 2. Generate SeatTemplates for 16-seat
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '11000000-0000-0000-0000-000000000016')
BEGIN
    DECLARE @i16 INT = 1, @r16 INT = 1, @c16 INT = 1;
    WHILE @i16 <= 16
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (NEWID(), '11000000-0000-0000-0000-000000000016', 'S' + RIGHT('00'+CAST(@i16 AS VARCHAR),2), @r16, @c16, 1, 0);
        SET @c16 = @c16 + 1;
        IF @c16 > 4 BEGIN SET @c16 = 1; SET @r16 = @r16 + 1; END
        SET @i16 = @i16 + 1;
    END
END;

-- 3. Generate SeatTemplates for 29-seat
IF NOT EXISTS (SELECT 1 FROM SeatTemplates WHERE BusTypeId = '11000000-0000-0000-0000-000000000029')
BEGIN
    DECLARE @i29 INT = 1, @r29 INT = 1, @c29 INT = 1;
    WHILE @i29 <= 29
    BEGIN
        INSERT INTO SeatTemplates (Id, BusTypeId, SeatNumber, RowNumber, ColumnNumber, Floor, Type)
        VALUES (NEWID(), '11000000-0000-0000-0000-000000000029', 'S' + RIGHT('00'+CAST(@i29 AS VARCHAR),2), @r29, @c29, 1, 0);
        SET @c29 = @c29 + 1;
        IF @c29 > 4 BEGIN SET @c29 = 1; SET @r29 = @r29 + 1; END
        SET @i29 = @i29 + 1;
    END
END;

PRINT 'BusTypes and SeatTemplates seeded successfully!';
GO
