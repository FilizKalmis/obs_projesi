-- 2. SP: Gözetmen Havuzundan Otomatik Atama (Adil Daðýtým Ýlkesiyle)
CREATE PROCEDURE sp_GozetmenHavuzundanAta
    @SinavSalonuID INT
AS
BEGIN
    -- En az görev almýþ, mazereti olmayan ve 3 oturum kuralýna takýlmayan hocayý bul
    INSERT INTO GozetmenAtama (SinavSalonuID, PersonelID)
    SELECT TOP 1 @SinavSalonuID, P.PersonelID
    FROM Personel P
    LEFT JOIN (SELECT PersonelID, COUNT(*) as GorevSayisi FROM GozetmenAtama GROUP BY PersonelID) G 
        ON P.PersonelID = G.PersonelID
    WHERE dbo.fn_GozetmenOturumSiniri(P.PersonelID, GETDATE(), 1) = 0 -- Fonksiyonu kullanýyoruz
    ORDER BY G.GorevSayisi ASC; -- Az görev alana öncelik ver
END;