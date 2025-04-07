using Data.Models;

namespace Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<DbResult<IEnumerable<UserModel>>> GetAllAsync();
    Task<DbResult<UserModel?>> GetByIdAsync(int userId);
    Task<DbResult<UserModel?>> GetByEmailAsync(string email);
    Task<DbResult<int?>> AddAsync(UserModel userModel);
    Task<DbResult<bool>> SoftDeleteAsync(int userId);
    
    Task<DbResult<bool>> ChangeUsernameAsync(int userId, string newUsername);
    Task<DbResult<bool>> ChangeEmailAsync(int userId, string newEmail);
    Task<DbResult<bool>> ChangePasswordAsync(int userId, string newPasswordHash);
}
