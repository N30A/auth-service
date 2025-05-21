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
            TokenHash VARCHAR(255) NOT NULL,
            UserId UNIQUEIDENTIFIER NOT NULL REFERENCES Users(UserId),
            IssuedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
            ExpiresAt DATETIME2 NOT NULL,
            RevokedAt DATETIME2 NULL,
            UserAgent NVARCHAR(255) NULL,
            IpAddress NVARCHAR(39) NULL,
            IsUsed BIT NOT NULL DEFAULT 0
        );
    END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'Index_RefreshTokens_UserId')
    BEGIN
        CREATE INDEX Index_RefreshTokens_UserId ON RefreshTokens(UserId);
    END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'Index_RefreshTokens_Token')
    BEGIN
        CREATE INDEX Index_RefreshTokens_Token ON RefreshTokens(TokenHash);
    END
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
