-- 3. SP: Veritabaný Yedekle (Bonus Ýster)
CREATE PROCEDURE sp_VeritabaniYedekle
    @Yol NVARCHAR(255)
AS
BEGIN
    BACKUP DATABASE [OBS_Projesi] 
    TO DISK = @Yol WITH FORMAT, MEDIANAME = 'OBS_Bak', NAME = 'Full Backup of OBS';
END;