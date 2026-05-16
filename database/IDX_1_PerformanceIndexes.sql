/*
    IDX_1_PerformanceIndexes.sql

    Sınav planlama, salon atama, gözetmen atama ve mazeret kontrollerinde
    sık kullanılan alanlar için performans indexleri.
*/

------------------------------------------------------------
-- 1. Sinav: Tarih + OturumID
------------------------------------------------------------

IF OBJECT_ID(N'dbo.Sinav', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Sinav_Tarih_OturumID'
      AND object_id = OBJECT_ID(N'dbo.Sinav')
)
BEGIN
    CREATE INDEX IX_Sinav_Tarih_OturumID
    ON dbo.Sinav (Tarih, OturumID);
END
GO

------------------------------------------------------------
-- 2. Ders: Yariyil + BolumID
------------------------------------------------------------

IF OBJECT_ID(N'dbo.Ders', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Ders_Yariyil_BolumID'
      AND object_id = OBJECT_ID(N'dbo.Ders')
)
BEGIN
    CREATE INDEX IX_Ders_Yariyil_BolumID
    ON dbo.Ders (Yariyil, BolumID);
END
GO

------------------------------------------------------------
-- 3. SinavSalonu: DerslikID
------------------------------------------------------------

IF OBJECT_ID(N'dbo.SinavSalonu', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_SinavSalonu_DerslikID'
      AND object_id = OBJECT_ID(N'dbo.SinavSalonu')
)
BEGIN
    CREATE INDEX IX_SinavSalonu_DerslikID
    ON dbo.SinavSalonu (DerslikID);
END
GO

------------------------------------------------------------
-- 4. GozetmenAtama: PersonelID
------------------------------------------------------------

IF OBJECT_ID(N'dbo.GozetmenAtama', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_GozetmenAtama_PersonelID'
      AND object_id = OBJECT_ID(N'dbo.GozetmenAtama')
)
BEGIN
    CREATE INDEX IX_GozetmenAtama_PersonelID
    ON dbo.GozetmenAtama (PersonelID);
END
GO

------------------------------------------------------------
-- 5. PersonelMazeret: PersonelID + Tarih + OturumID
------------------------------------------------------------

IF OBJECT_ID(N'dbo.PersonelMazeret', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_PersonelMazeret_PersonelID_Tarih_OturumID'
      AND object_id = OBJECT_ID(N'dbo.PersonelMazeret')
)
BEGIN
    CREATE INDEX IX_PersonelMazeret_PersonelID_Tarih_OturumID
    ON dbo.PersonelMazeret (PersonelID, Tarih, OturumID);
END
GO

------------------------------------------------------------
-- 6. Ek faydalı indexler
-- Bağlantılı kayıtları ararken ve silerken işimize yarar.
------------------------------------------------------------

IF OBJECT_ID(N'dbo.SinavSalonu', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_SinavSalonu_SinavID'
      AND object_id = OBJECT_ID(N'dbo.SinavSalonu')
)
BEGIN
    CREATE INDEX IX_SinavSalonu_SinavID
    ON dbo.SinavSalonu (SinavID);
END
GO

IF OBJECT_ID(N'dbo.GozetmenAtama', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_GozetmenAtama_SinavSalonuID'
      AND object_id = OBJECT_ID(N'dbo.GozetmenAtama')
)
BEGIN
    CREATE INDEX IX_GozetmenAtama_SinavSalonuID
    ON dbo.GozetmenAtama (SinavSalonuID);
END
GO

------------------------------------------------------------
-- 7. Kontrol sorgusu
------------------------------------------------------------

SELECT 
    t.name AS TabloAdi,
    i.name AS IndexAdi,
    i.type_desc AS IndexTuru
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name IN
(
    N'IX_Sinav_Tarih_OturumID',
    N'IX_Ders_Yariyil_BolumID',
    N'IX_SinavSalonu_DerslikID',
    N'IX_GozetmenAtama_PersonelID',
    N'IX_PersonelMazeret_PersonelID_Tarih_OturumID',
    N'IX_SinavSalonu_SinavID',
    N'IX_GozetmenAtama_SinavSalonuID'
)
ORDER BY t.name, i.name;
GO