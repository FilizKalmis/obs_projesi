CREATE TABLE Sinav (
    SinavID INT IDENTITY(1,1) PRIMARY KEY,
    DersID INT,
    Tarih DATE,
    OturumID INT,
    FOREIGN KEY (DersID) REFERENCES Ders(DersID),
    FOREIGN KEY (OturumID) REFERENCES Oturum(OturumID)
);