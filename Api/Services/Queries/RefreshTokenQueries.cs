namespace Api.Services.Queries;

public static class RefreshTokenQueries
{
    public const string Add = """
        INSERT INTO RefreshTokens (
            TokenHash,
            UserId,
            IssuedAt,
            ExpiresAt,
            RevokedAt,
            UserAgent,
            IpAddress,
            IsUsed
        )
        VALUES (
            @TokenHash,
            @UserId,
            @IssuedAt,
            @ExpiresAt,
            @RevokedAt,
            @UserAgent,
            @IpAddress,
            @IsUsed
        );
    """;
}