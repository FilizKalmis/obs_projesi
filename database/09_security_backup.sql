CREATE TABLE GozetmenAtama (
    AtamaID INT,
    PersonelID INT,
    PRIMARY KEY (AtamaID, PersonelID),
    FOREIGN KEY (AtamaID) REFERENCES SinavSalon(AtamaID),
    FOREIGN KEY (PersonelID) REFERENCES Personel(PersonelID)
);