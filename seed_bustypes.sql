-- Insert missing BusTypes
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '11000000-0000-0000-0000-000000000016')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('11000000-0000-0000-0000-000000000016', N'Xe ghế ngồi (16 chỗ)', 16, 'Standard Normal Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '11000000-0000-0000-0000-000000000029')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('11000000-0000-0000-0000-000000000029', N'Xe ghế ngồi (29 chỗ)', 29, 'Standard Normal Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '22000000-0000-0000-0000-000000000011')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('22000000-0000-0000-0000-000000000011', N'Xe Limousine (11 chỗ)', 11, 'VIP Limousine Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '22000000-0000-0000-0000-000000000016')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('22000000-0000-0000-0000-000000000016', N'Xe Limousine (16 chỗ)', 16, 'VIP Limousine Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '22000000-0000-0000-0000-000000000019')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('22000000-0000-0000-0000-000000000019', N'Xe Limousine (19 chỗ)', 19, 'VIP Limousine Seat');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '33000000-0000-0000-0000-000000000034')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('33000000-0000-0000-0000-000000000034', N'Xe giường nằm (34 giường)', 34, 'Standard Sleeper');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '55000000-0000-0000-0000-000000000020')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('55000000-0000-0000-0000-000000000020', N'Xe giường phòng (20 phòng đơn)', 20, 'Cabin Single');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '55000000-0000-0000-0000-000000000024')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('55000000-0000-0000-0000-000000000024', N'Xe giường phòng (24 phòng đơn)', 24, 'Cabin Single');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '66000000-0000-0000-0000-000000000022')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('66000000-0000-0000-0000-000000000022', N'Xe giường phòng (22 phòng đôi)', 22, 'Cabin Double');

IF NOT EXISTS (SELECT 1 FROM BusTypes WHERE Id = '66000000-0000-0000-0000-000000000024')
INSERT INTO BusTypes (Id, Name, SeatCount, Description) VALUES ('66000000-0000-0000-0000-000000000024', N'Xe giường phòng (24 phòng đôi)', 24, 'Cabin Double');

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
