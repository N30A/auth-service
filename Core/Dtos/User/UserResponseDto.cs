namespace Core.Dtos.User;

public class UserResponseDto
{
    public required int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}