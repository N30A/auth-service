namespace Core.Dtos.User;

public class AddUserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}