CREATE TABLE SinavSalon (
    AtamaID INT IDENTITY(1,1) PRIMARY KEY,
    SinavID INT,
    DerslikID INT,
    FOREIGN KEY (SinavID) REFERENCES Sinav(SinavID),
    FOREIGN KEY (DerslikID) REFERENCES Derslik(DerslikID)
);