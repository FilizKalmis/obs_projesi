CREATE TABLE Oturum (
    OturumID INT IDENTITY(1,1) PRIMARY KEY,
    Aciklama NVARCHAR(50),
    BaslangicSaat TIME NOT NULL,
    BitisSaat TIME NOT NULL
);