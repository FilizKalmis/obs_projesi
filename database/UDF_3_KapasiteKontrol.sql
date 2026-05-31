-- 3. Fonksiyon: Atanan salonlarýn toplam kapasitesi yeterli mi?
CREATE FUNCTION fn_KapasiteKontrol(@SinavID INT)
RETURNS BIT
AS
BEGIN
    DECLARE @Gereken INT, @Mevcut INT;
    
    -- Dersteki öðrenci sayýsýný al
    SELECT @Gereken = D.OgrenciSayisi FROM Sinav S 
    JOIN Ders D ON S.DersID = D.DersID WHERE S.SinavID = @SinavID;
    
    -- Atanan salonlarýn toplam kapasitesini hesapla
    SELECT @Mevcut = SUM(Dsl.Kapasite) FROM SinavSalonu SS
    JOIN Derslik Dsl ON SS.DerslikID = Dsl.DerslikID
    WHERE SS.SinavID = @SinavID;

    IF @Mevcut >= @Gereken RETURN 1; -- Kapasite yeterli
    RETURN 0; -- Kapasite yetersiz
END;