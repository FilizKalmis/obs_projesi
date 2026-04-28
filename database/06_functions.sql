CREATE TABLE PersonelMazeret (
    MazeretID INT IDENTITY(1,1) PRIMARY KEY,
    PersonelID INT,
    Tarih DATE,
    MazeretTuru NVARCHAR(100),
    Uygun BIT,
    FOREIGN KEY (PersonelID) REFERENCES Personel(PersonelID)
);