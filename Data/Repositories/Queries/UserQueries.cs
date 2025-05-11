namespace Data.Repositories.Queries;

public static class UserQueries
{
    public const string GetAll = """
        SELECT
            UserId, Username, Email, CreatedAt, UpdatedAt, DeletedAt
        FROM [User];
    """;
    
    public const string GetById = """
        SELECT
            UserId, Username, Email, CreatedAt, UpdatedAt, DeletedAt
        FROM [User]
        WHERE UserId = @UserId;
    """;
    
    public const string GetByEmail = """
        SELECT
            UserId, Username, Email, PasswordHash, CreatedAt, UpdatedAt, DeletedAt
        FROM [User]
        WHERE Email = @Email;
    """;
    
    public const string Add = """
        INSERT INTO [User](Username, Email, PasswordHash)
        OUTPUT INSERTED.UserId, INSERTED.Username, INSERTED.Email, INSERTED.CreatedAt, INSERTED.UpdatedAt, INSERTED.DeletedAt
        VALUES(@Username, @Email, @PasswordHash);
    """;
    
    public const string SoftDelete = """
        UPDATE [User]
        SET DeletedAt = GETDATE()
        WHERE UserId = @UserId;
    """;
    
    public const string Restore = """
        UPDATE [User]
        SET DeletedAt = null
        WHERE UserId = @UserId AND DeletedAt IS NOT NULL;
    """;
    
    public const string ChangeUsername = """
        UPDATE [User]
        SET Username = @NewUsername
        WHERE UserId = @UserId;
    """;
    
    public const string ChangeEmail = """
        UPDATE [User]
        SET Email = @NewEmail
        WHERE UserId = @UserId;
    """;
    
    public const string ChangePassword = """
        UPDATE [User]
        SET PasswordHash = @NewPasswordHash
        WHERE UserId = @UserId;
    """;
}
