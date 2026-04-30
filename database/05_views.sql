CREATE TABLE Ders (
    DersID INT IDENTITY(1,1) PRIMARY KEY,
    DersKodu NVARCHAR(20),
    DersAdi NVARCHAR(100),
    DersTuru NVARCHAR(50),
    OgrenciSayisi INT,
    Yariyil INT,
    BolumID INT,
    FOREIGN KEY (BolumID) REFERENCES Bolum(BolumID)
);