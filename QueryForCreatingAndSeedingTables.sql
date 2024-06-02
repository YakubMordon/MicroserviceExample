CREATE TABLE Users (
    Id INT PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    Role NVARCHAR(50) NOT NULL
);

INSERT INTO Users (Id, Username, Password, Role)
VALUES (1, 'admin', 'admin', 'admin'),
       (2, 'user', 'user', 'user'),
       (3, 'user2', 'user2', 'user');

CREATE TABLE Cars (
    Id INT PRIMARY KEY,
    Brand NVARCHAR(50) NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    IsAvailable BIT NOT NULL
);

INSERT INTO Cars (Id, Brand, Model, IsAvailable)
VALUES (1, 'Audi', 'RS6', 1),
       (2, 'BMW', 'M5', 1),
       (3, 'Mercedes-Benz', 'E-class', 1),
       (4, 'Audi', 'RS2', 0),
       (5, 'Ford', 'Mustang', 1);

CREATE TABLE Orders (
    Id INT PRIMARY KEY,
    CarId INT NOT NULL,
    UserId INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    ReturnDate DATETIME,
    IsCompleted BIT NOT NULL,
    FOREIGN KEY (CarId) REFERENCES Cars(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

INSERT INTO Orders (Id, CarId, UserId, OrderDate, ReturnDate, IsCompleted)
VALUES (1, 4, 1, GETDATE(), NULL, 0),
       (2, 2, 2, GETDATE(), NULL, 0),
       (3, 1, 3, GETDATE(), NULL, 0);

CREATE TABLE Returns (
    Id INT PRIMARY KEY,
    CarId INT NOT NULL,
    IsDamaged BIT NOT NULL,
    ReturnDate DATETIME NOT NULL,
    FOREIGN KEY (CarId) REFERENCES Cars(Id)
);

INSERT INTO Returns (Id, CarId, IsDamaged, ReturnDate)
VALUES (1, 2, 1, GETDATE()),
       (2, 5, 1, GETDATE());

CREATE TABLE Payments (
    Id INT PRIMARY KEY,
    OrderId INT NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    PaymentDate DATETIME NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
);

INSERT INTO Payments (Id, OrderId, Amount, PaymentDate)
VALUES (1, 1, 1, GETDATE()),
       (2, 2, 1, GETDATE()),
       (3, 3, 1, GETDATE());
