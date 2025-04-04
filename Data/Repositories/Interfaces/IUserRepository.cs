namespace Data.Repositories.Interfaces;

using Models;

public interface IUserRepository
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int userId);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<int?> AddAsync(UserDto userDto);
    Task<bool> SoftDeleteAsync(int userId);
    
    Task<bool> ChangeUsernameAsync(int userId, string newUsername);
    Task<bool> ChangeEmailAsync(int userId, string newEmail);
    Task<bool> ChangePasswordAsync(int userId, string newPasswordHash);
}