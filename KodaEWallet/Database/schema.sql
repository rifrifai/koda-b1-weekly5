IF DB_ID('KodaEWallet') IS NULL
    CREATE DATABASE KodaEWallet;
GO

-- USE KodaEWallet;
-- GO

-- DROP TABLE IF EXISTS Transactions;
-- DROP TABLE IF EXISTS Wallets;
-- DROP TABLE IF EXISTS Users;
-- GO

-- CREATE TABLE Users (
--     Id    INT IDENTITY(1,1) NOT NULL,
--     Name  NVARCHAR(100) NOT NULL,
--     Email NVARCHAR(150) NOT NULL,
--     Phone NVARCHAR(20)  NOT NULL,
--     CONSTRAINT PK_Users PRIMARY KEY (Id),
--     CONSTRAINT UQ_Users_Email UNIQUE (Email)
-- );

-- CREATE TABLE Wallets (
--     Id           INT IDENTITY(1,1) NOT NULL,
--     UserId       INT NOT NULL,
--     WalletNumber NVARCHAR(20) NOT NULL,
--     Balance      DECIMAL(18,2) NOT NULL CONSTRAINT DF_Wallets_Balance DEFAULT 0,
--     CONSTRAINT PK_Wallets PRIMARY KEY (Id),
--     CONSTRAINT UQ_Wallets_UserId UNIQUE (UserId),
--     CONSTRAINT UQ_Wallets_WalletNumber UNIQUE (WalletNumber),
--     CONSTRAINT FK_Wallets_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
--     CONSTRAINT CK_Wallets_Balance CHECK (Balance >= 0)   -- saldo tidak boleh minus
-- );

-- CREATE TABLE Transactions (
--     Id               INT IDENTITY(1,1) NOT NULL,
--     SenderWalletId   INT NULL,
--     ReceiverWalletId INT NOT NULL,
--     Type             NVARCHAR(20) NOT NULL,
--     Amount           DECIMAL(18,2) NOT NULL,
--     Description      NVARCHAR(200) NULL,
--     CreatedAt        DATETIME2 NOT NULL CONSTRAINT DF_Transactions_CreatedAt DEFAULT SYSDATETIME(),
--     CONSTRAINT PK_Transactions PRIMARY KEY (Id),
--     CONSTRAINT FK_Transactions_Sender   FOREIGN KEY (SenderWalletId)   REFERENCES Wallets(Id),
--     CONSTRAINT FK_Transactions_Receiver FOREIGN KEY (ReceiverWalletId) REFERENCES Wallets(Id),
--     CONSTRAINT CK_Transactions_Type   CHECK (Type IN ('TOPUP', 'TRANSFER')),
--     CONSTRAINT CK_Transactions_Amount CHECK (Amount > 0),
--     CONSTRAINT CK_Transactions_Shape  CHECK (
--         (Type = 'TOPUP'    AND SenderWalletId IS NULL) OR
--         (Type = 'TRANSFER' AND SenderWalletId IS NOT NULL AND SenderWalletId <> ReceiverWalletId))
-- );

-- CREATE INDEX IX_Transactions_SenderWalletId   ON Transactions (SenderWalletId, CreatedAt DESC);
-- CREATE INDEX IX_Transactions_ReceiverWalletId ON Transactions (ReceiverWalletId, CreatedAt DESC);
-- GO

-- INSERT INTO Users (Name, Email, Phone) VALUES
--     ('Andi',  'andi@mail.com',  '081111111111'),
--     ('Budi',  'budi@mail.com',  '082222222222'),
--     ('Citra', 'citra@mail.com', '083333333333');

-- INSERT INTO Wallets (UserId, WalletNumber, Balance) VALUES
--     (1, 'WAL001', 500000),
--     (2, 'WAL002', 350000),
--     (3, 'WAL003', 350000);

-- INSERT INTO Transactions (SenderWalletId, ReceiverWalletId, Type, Amount, Description, CreatedAt) VALUES
--     (NULL, 1, 'TOPUP',    200000, 'Top up awal',    '2026-10-28 09:00'),
--     (NULL, 2, 'TOPUP',    400000, 'Top up awal',    '2026-10-28 10:30'),
--     (NULL, 3, 'TOPUP',    600000, 'Top up awal',    '2026-10-29 08:15'),
--     (3,    1, 'TRANSFER', 300000, 'Traktir makan warung gatcha', '2026-10-30 13:45'),
--     (2,    3, 'TRANSFER',  50000, 'Bayar hutang kas',      '2026-11-01 19:20');
-- GO
SELECT * FROM Transactions;
