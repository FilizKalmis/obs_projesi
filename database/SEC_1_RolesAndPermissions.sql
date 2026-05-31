/*
    SEC_1_RolesAndPermissions.sql

    Bu dosya iki uygulama kullanıcısı oluşturur:

    1. App_Admin
       - Tüm tablolarda okuma, ekleme, güncelleme ve silme yetkisine sahiptir.

    2. App_Viewer
       - Sadece rapor görüntüleme yetkisine sahiptir.
       - Veri ekleyemez, güncelleyemez, silemez.

    Not:
    Bu scripti çalıştırmadan önce SQL Server'da proje veritabanını seç.
*/

------------------------------------------------------------
-- 1. Server login kayıtları
------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1
    FROM sys.server_principals
    WHERE name = N'App_Admin'
)
BEGIN
    CREATE LOGIN App_Admin
    WITH PASSWORD = 'Admin_12345!',
         CHECK_POLICY = OFF,
         CHECK_EXPIRATION = OFF;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.server_principals
    WHERE name = N'App_Viewer'
)
BEGIN
    CREATE LOGIN App_Viewer
    WITH PASSWORD = 'Viewer_12345!',
         CHECK_POLICY = OFF,
         CHECK_EXPIRATION = OFF;
END
GO

------------------------------------------------------------
-- 2. Veritabanı kullanıcıları
------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = N'App_Admin'
)
BEGIN
    CREATE USER App_Admin FOR LOGIN App_Admin;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = N'App_Viewer'
)
BEGIN
    CREATE USER App_Viewer FOR LOGIN App_Viewer;
END
GO

------------------------------------------------------------
-- 3. Veritabanı rolleri
------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = N'AppAdminRole'
      AND type = 'R'
)
BEGIN
    CREATE ROLE AppAdminRole;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = N'AppViewerRole'
      AND type = 'R'
)
BEGIN
    CREATE ROLE AppViewerRole;
END
GO

------------------------------------------------------------
-- 4. Kullanıcıları rollere ekleme
------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members rm
    INNER JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
    INNER JOIN sys.database_principals u ON rm.member_principal_id = u.principal_id
    WHERE r.name = N'AppAdminRole'
      AND u.name = N'App_Admin'
)
BEGIN
    ALTER ROLE AppAdminRole ADD MEMBER App_Admin;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members rm
    INNER JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
    INNER JOIN sys.database_principals u ON rm.member_principal_id = u.principal_id
    WHERE r.name = N'AppViewerRole'
      AND u.name = N'App_Viewer'
)
BEGIN
    ALTER ROLE AppViewerRole ADD MEMBER App_Viewer;
END
GO

------------------------------------------------------------
-- 5. Önce eski yetkileri temizleme
------------------------------------------------------------

REVOKE SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo FROM AppAdminRole;
REVOKE SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo FROM AppViewerRole;
GO

------------------------------------------------------------
-- 6. App_Admin yetkileri
------------------------------------------------------------

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO AppAdminRole;
GO

------------------------------------------------------------
-- 7. App_Viewer yetkileri
-- Sadece rapor ekranlarında kullanılan görünümlere okuma izni verilir.
------------------------------------------------------------

IF OBJECT_ID(N'dbo.vw_SinavProgramiDetay', N'V') IS NOT NULL
BEGIN
    GRANT SELECT ON OBJECT::dbo.vw_SinavProgramiDetay TO AppViewerRole;
END
GO

IF OBJECT_ID(N'dbo.vw_DerslikDurumu', N'V') IS NOT NULL
BEGIN
    GRANT SELECT ON OBJECT::dbo.vw_DerslikDurumu TO AppViewerRole;
END
GO

IF OBJECT_ID(N'dbo.vw_GozetmenGorevYuku', N'V') IS NOT NULL
BEGIN
    GRANT SELECT ON OBJECT::dbo.vw_GozetmenGorevYuku TO AppViewerRole;
END
GO

------------------------------------------------------------
-- 8. App_Viewer veri değiştiremesin
------------------------------------------------------------

DENY INSERT, UPDATE, DELETE ON SCHEMA::dbo TO AppViewerRole;
GO

------------------------------------------------------------
-- 9. Kontrol sorguları
------------------------------------------------------------

SELECT 
    dp.name AS KullaniciAdi,
    dp.type_desc AS KullaniciTipi
FROM sys.database_principals dp
WHERE dp.name IN (N'App_Admin', N'App_Viewer', N'AppAdminRole', N'AppViewerRole');

SELECT 
    r.name AS RolAdi,
    u.name AS KullaniciAdi
FROM sys.database_role_members rm
INNER JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
INNER JOIN sys.database_principals u ON rm.member_principal_id = u.principal_id
WHERE r.name IN (N'AppAdminRole', N'AppViewerRole');