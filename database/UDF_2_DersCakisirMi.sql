-- 2. Fonksiyon: Aynı yarıyıl dersleri çakışıyor mu?
CREATE FUNCTION fn_YariyilCakismaKontrol(@Yariyil INT, @Tarih DATE, @OturumID INT)
RETURNS BIT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Sinav S JOIN Ders D ON S.DersID = D.DersID 
               WHERE D.Yariyil = @Yariyil AND S.Tarih = @Tarih AND S.OturumID = @OturumID)
        RETURN 1; -- Çakışma var
    RETURN 0;
END;