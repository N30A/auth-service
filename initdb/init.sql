IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'AuthService')
    BEGIN
        CREATE DATABASE AuthService;
    END;
GO

USE AuthService;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
    BEGIN
        CREATE TABLE Users (
           UserId UNIQUEIDENTIFIER PRIMARY KEY,
           Username VARCHAR(20) UNIQUE NOT NULL,
           Email VARCHAR(254) UNIQUE NOT NULL,
           PasswordHash VARCHAR(256) NOT NULL,
           CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
           UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
           DeletedAt DATETIME2 DEFAULT NULL
        );
    END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Roles')
    BEGIN
        CREATE TABLE Roles (
           RoleId INT IDENTITY PRIMARY KEY,
           Name VARCHAR(20) UNIQUE NOT NULL
        );
    END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserRoles')
    BEGIN
        CREATE TABLE UserRoles (
           UserId UNIQUEIDENTIFIER REFERENCES Users(UserId),
           RoleId INT REFERENCES Roles(RoleId),
           PRIMARY KEY (UserId, RoleId)
        );
    END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RefreshTokens')
    BEGIN
        CREATE TABLE RefreshTokens (
           RefreshTokenId INT IDENTITY PRIMARY KEY,
           Token VARCHAR(2048) NOT NULL,
           UserId UNIQUEIDENTIFIER REFERENCES Users(UserId),
           IssuedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
           ExpiresAt DATETIME2 NOT NULL
        );
    END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.triggers WHERE name = 'trg_Users_UpdatedAt'
)
    BEGIN
        EXEC('
            CREATE TRIGGER trg_Users_UpdatedAt
                ON Users
                AFTER UPDATE AS
            BEGIN
                UPDATE Users
                SET UpdatedAt = SYSUTCDATETIME()
                FROM Users
                INNER JOIN inserted ON Users.UserId = inserted.UserId;
            END;
            ');
    END;
GO

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
    BEGIN
        INSERT INTO Roles (Name) VALUES ('Admin');
    END;
GO
