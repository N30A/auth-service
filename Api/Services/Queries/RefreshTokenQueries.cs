namespace Api.Services.Queries;

public static class RefreshTokenQueries
{
    public const string Add = """
        INSERT INTO RefreshTokens (
            TokenHash, UserId, IssuedAt, ExpiresAt,
            RevokedAt, UserAgent, IpAddress, IsUsed    
        )
        VALUES (
            @TokenHash, @UserId, @IssuedAt, @ExpiresAt,
            @RevokedAt, @UserAgent, @IpAddress, @IsUsed
        );
    """;

    public const string GetByHash = """
        SELECT
            RefreshTokenId, TokenHash, UserId, IssuedAt, ExpiresAt,
            RevokedAt, UserAgent, IpAddress, IsUsed    
        FROM RefreshTokens
        WHERE TokenHash = @TokenHash;
    """;

    public const string SetRevoked = """
    UPDATE RefreshTokens
        SET RevokedAt = SYSUTCDATETIME()
        WHERE RefreshTokenId = @RefreshTokenId;
    """;
}
