namespace Core.PasswordHasher;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool Verify(string hashedPassword, string password);
}
