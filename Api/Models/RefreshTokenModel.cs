namespace Api.Models;

public class RefreshTokenModel
{   
    public string RefreshTokenId { get; set; }
    public string Token { get; set; }
    public string TokenHash { get; set; }
    public Guid UserId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public bool IsUsed { get; set; }
}