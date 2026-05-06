USE [OBS_SınavSistemi];
GO

INSERT INTO Bolum (BolumAdi)
VALUES
('Makine Mühendisligi'),
('Yazilim Muhendisligi'),
('İnşaat Mühendisliği');

SELECT * FROM Bolum;

USE [OBS_SınavSistemi];
GO

DECLARE @MakineID INT = (
    SELECT BolumID FROM Bolum WHERE BolumAdi = 'Makine Mühendisligi'
);

INSERT INTO Personel (Unvan, Ad, Soyad, BolumID) 
VALUES 
('Prof. Dr.', 'Caner', 'Guven', @MakineID),
('Dr. Ogr. Uyesi', 'Selin', 'Sizma', @MakineID);


DECLARE @YazilimID INT = (
    SELECT BolumID FROM Bolum WHERE BolumAdi = 'Yazilim Muhendisligi'
);

INSERT INTO Personel (Unvan, Ad, Soyad, BolumID) 
VALUES 
('Doc. Dr.', 'Murat', 'Kodlu', @YazilimID),
('Ars. Gor.', 'Zeynep', 'Derle', @YazilimID);


DECLARE @InsaatID INT = (
    SELECT BolumID FROM Bolum WHERE BolumAdi = 'İnşaat Mühendisliği'
);

INSERT INTO Personel (Unvan, Ad, Soyad, BolumID) 
VALUES 
('Ogr. Gor.', 'Hakan', 'Analiz', @InsaatID);


SELECT P.PersonelID, P.Unvan, P.Ad, P.Soyad, B.BolumAdi
FROM Personel P
JOIN Bolum B ON P.BolumID = B.BolumID
ORDER BY B.BolumAdi;

USE [OBS_SınavSistemi];
GO

-- Admin kullanıcısı ekle
INSERT INTO Kullanici (KullaniciAdi, Sifre, Rol, PersonelID) 
VALUES ('admin', '12345', 'Admin', 1);

-- Zeynep kişisini Viewer olarak ekle
DECLARE @GozetmenPersonelID INT = (
    SELECT PersonelID 
    FROM Personel 
    WHERE Ad = 'Zeynep' AND Soyad = 'Derle'
);

INSERT INTO Kullanici (KullaniciAdi, Sifre, Rol, PersonelID) 
VALUES ('zeynep_gozetmen', '54321', 'Viewer', @GozetmenPersonelID);

SELECT * FROM Kullanici;