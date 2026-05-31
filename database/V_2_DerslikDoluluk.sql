-- 2. Görünüm: Derslik Doluluk Oranları
CREATE VIEW vw_DerslikDurumu AS
SELECT Dl.Ad, Dl.Kapasite, COUNT(SS.SinavID) as ToplamSinavSayisi
FROM Derslik Dl
LEFT JOIN SinavSalonu SS ON Dl.DerslikID = SS.DerslikID
GROUP BY Dl.Ad, Dl.Kapasite;

