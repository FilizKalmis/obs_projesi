-- 2. Trigger: Sýnav saati deðiþtiðinde Log kaydý oluþtur
CREATE TRIGGER trg_SinavGuncellemeLog
ON Sinav
AFTER UPDATE
AS
BEGIN
    IF UPDATE(OturumID)
    BEGIN
        INSERT INTO SinavLog (SinavID, IslemTuru, EskiDeger, YeniDeger, PersonelID, IslemTarihi)
        SELECT i.SinavID, 'Oturum Guncelleme', 
             CAST(d.OturumID AS NVARCHAR), 
            CAST(i.OturumID AS NVARCHAR), 
            1, -- PersonelID bir INT olmalý. Sistemdeki Admin ID'si 1 ise 1 yazýyoruz
            GETDATE()
        FROM inserted i
        JOIN deleted d ON i.SinavID = d.SinavID;
    END
END;