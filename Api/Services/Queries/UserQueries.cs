namespace Api.Services.Queries;

public static class UserQueries
{
    public const string GetAll = """
        SELECT
            UserId, Username, Email, CreatedAt, UpdatedAt, DeletedAt
        FROM Users;
    """;
    
    public const string GetById = """
        SELECT
            UserId, Username, Email, CreatedAt, UpdatedAt, DeletedAt
        FROM Users
        WHERE UserId = @UserId;
    """;
    
    public const string GetByEmail = """
        SELECT
            UserId, Username, Email, PasswordHash, CreatedAt, UpdatedAt, DeletedAt
        FROM Users
        WHERE Email = @Email;
    """;
    
    public const string Add = """
        INSERT INTO Users(UserId, Username, Email, PasswordHash)
        OUTPUT INSERTED.UserId, INSERTED.Username, INSERTED.Email, INSERTED.CreatedAt, INSERTED.UpdatedAt, INSERTED.DeletedAt
        VALUES(@UserId, @Username, @Email, @PasswordHash);
    """;
    
    public const string SoftDelete = """
        UPDATE Users
        SET DeletedAt = GETDATE()
        WHERE UserId = @UserId;
    """;
    
    public const string Restore = """
        UPDATE Users
        SET DeletedAt = null
        WHERE UserId = @UserId AND DeletedAt IS NOT NULL;
    """;

    public const string Update = """
        DECLARE @UpdatedUsers TABLE (
            UserId UNIQUEIDENTIFIER,
            Username VARCHAR(20),
            Email VARCHAR(254),
            CreatedAt DATETIME2,
            UpdatedAt DATETIME2,
            DeletedAt DATETIME2
        );

        UPDATE Users
        SET
            Username = COALESCE(@Username, Username),
            Email = COALESCE(@Email, Email)
        OUTPUT
            INSERTED.UserId,
            INSERTED.Username,
            INSERTED.Email,
            INSERTED.CreatedAt,
            INSERTED.UpdatedAt,
            INSERTED.DeletedAt
            INTO @UpdatedUsers
        WHERE UserId = @UserId;
        
        SELECT * FROM @UpdatedUsers;
     """;
}
