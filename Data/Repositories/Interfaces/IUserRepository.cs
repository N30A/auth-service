namespace Data.Repositories.Interfaces;

using Models;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByEmailAsync(string email);
    Task<int?> AddAsync(User user);
    Task<bool> SoftDeleteAsync(int userId);
    
    Task<bool> ChangeUsernameAsync(int userId, string newUsername);
    Task<bool> ChangeEmailAsync(int userId, string newEmail);
    Task<bool> ChangePasswordAsync(int userId, string newPasswordHash);
}