CREATE PROCEDURE sp_AkilliSalonAta
    @SinavID INT
AS
BEGIN
    DECLARE @Kontenjan INT, @KalanOgrenci INT;
    SELECT @Kontenjan = D.OgrenciSayisi FROM Sinav S 
    JOIN Ders D ON S.DersID = D.DersID WHERE S.SinavID = @SinavID;
    
    SET @KalanOgrenci = @Kontenjan;

    -- Kapasiteye göre salonlarý büyükten küçüðe bul ve ata (Basit versiyon)
    -- Ýleride Modül 2'deki "ayný kat" kuralý buraya eklenecek
    INSERT INTO SinavSalonu (SinavID, DerslikID)
    SELECT TOP 2 @SinavID, DerslikID FROM Derslik 
    WHERE Aktif = 1 ORDER BY Kapasite DESC;
END;