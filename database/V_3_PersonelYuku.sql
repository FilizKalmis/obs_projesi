-- 3. Görünüm: Personel Görev Yükü Tablosu[cite: 5]
CREATE VIEW vw_GozetmenGorevYuku AS
SELECT P.Unvan, P.Ad, P.Soyad, COUNT(GA.AtamaID) as GorevSayisi
FROM Personel P
LEFT JOIN GozetmenAtama GA ON P.PersonelID = GA.PersonelID
GROUP BY P.Unvan, P.Ad, P.Soyad;