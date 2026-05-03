-- 1. Fonksiyon: Gözetmen 3 oturum kuralýna takýlýyor mu?
CREATE FUNCTION fn_GozetmenOturumSiniri(@PersonelID INT, @Tarih DATE, @OturumID INT)
RETURNS BIT
AS
BEGIN
    DECLARE @ArdisikSayisi INT;
    -- Seçili oturumdan önceki 3 oturumda görevli mi kontrolü (Basit mantýk)
    SELECT @ArdisikSayisi = COUNT(*) 
    FROM GozetmenAtama GA
    JOIN SinavSalonu SS ON GA.SinavSalonuID = SS.SinavSalonuID
    JOIN Sinav S ON SS.SinavID = S.SinavID
    WHERE GA.PersonelID = @PersonelID AND S.Tarih = @Tarih 
    AND S.OturumID BETWEEN (@OturumID - 3) AND (@OturumID - 1);

    IF @ArdisikSayisi >= 3 RETURN 1; -- Yasak
    RETURN 0; -- Uygun
END;

