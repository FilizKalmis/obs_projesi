IF OBJECT_ID('trg_SinavGuncellemeLog', 'TR') IS NOT NULL
    DROP TRIGGER trg_SinavGuncellemeLog;
GO

CREATE TRIGGER trg_SinavGuncellemeLog
ON Sinav
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(Tarih) OR UPDATE(OturumID)
    BEGIN
        INSERT INTO SinavLog
        (
            SinavID,
            IslemTuru,
            EskiDeger,
            YeniDeger,
            PersonelID,
            IslemTarihi
        )
        SELECT
            i.SinavID,
            N'Sınav Güncelleme',

            N'Eski: ' +
            CONVERT(NVARCHAR(10), d.Tarih, 104) +
            N' / ' +
            ISNULL(od.Tanim, N'Oturum ' + CAST(d.OturumID AS NVARCHAR(20))),

            N'Yeni: ' +
            CONVERT(NVARCHAR(10), i.Tarih, 104) +
            N' / ' +
            ISNULL(oi.Tanim, N'Oturum ' + CAST(i.OturumID AS NVARCHAR(20))),

            1,
            GETDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.SinavID = d.SinavID
        LEFT JOIN Oturum od ON d.OturumID = od.OturumID
        LEFT JOIN Oturum oi ON i.OturumID = oi.OturumID
        WHERE
            ISNULL(i.Tarih, '19000101') <> ISNULL(d.Tarih, '19000101')
            OR ISNULL(i.OturumID, 0) <> ISNULL(d.OturumID, 0);
    END
END;
GO