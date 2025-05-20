namespace Api.PasswordHashers;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool Verify(string hashedPassword, string password);
}
