CREATE TRIGGER trg_SalonCakismaEngelle
ON SinavSalonu
AFTER INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN SinavSalonu ss ON i.DerslikID = ss.DerslikID
        JOIN Sinav s1 ON i.SinavID = s1.SinavID
        JOIN Sinav s2 ON ss.SinavID = s2.SinavID
        WHERE i.SinavSalonuID <> ss.SinavSalonuID AND s1.Tarih = s2.Tarih AND s1.OturumID = s2.OturumID
    )
    BEGIN
        RAISERROR('HATA: Bu derslik secilen oturumda zaten dolu!', 16, 1);
        ROLLBACK TRANSACTION; -- Ýþlemi geri al[cite: 5]
    END
END;