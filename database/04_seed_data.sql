CREATE TABLE Personel (
    PersonelID INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(50),
    Soyad NVARCHAR(50),
    Unvan NVARCHAR(50),
    BolumID INT,
    FOREIGN KEY (BolumID) REFERENCES Bolum(BolumID)
);