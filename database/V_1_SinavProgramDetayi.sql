-- 1. Görünüm: Genel Sınav Programı Detayı
CREATE VIEW vw_SinavProgramiDetay AS
SELECT 
    S.Tarih, 
    O.Tanim as Oturum, -- Artik 'O'  aşağıda tanımlı
    D.DersAdi, 
    Dl.Ad as Salon, 
    P.Ad + ' ' + P.Soyad as Gozetmen
FROM Sinav S
JOIN Oturum O ON S.OturumID = O.OturumID -- EKSİK OLAN SATIR BUYDU
JOIN Ders D ON S.DersID = D.DersID
JOIN SinavSalonu SS ON S.SinavID = SS.SinavID
JOIN Derslik Dl ON SS.DerslikID = Dl.DerslikID
LEFT JOIN GozetmenAtama GA ON SS.SinavSalonuID = GA.SinavSalonuID
LEFT JOIN Personel P ON GA.PersonelID = P.PersonelID;

