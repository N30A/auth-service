using Core.Dtos.User;

namespace Core.Services.Interfaces;

public interface IUserService
{
    public Task<Result<MultipleUsersDto>> GetAllAsync();
    Task<Result<UserDto?>> GetByIdAsync(int userId);
    Task<Result<UserDto?>> GetByEmailAsync(string email);
    Task<Result<bool>> SoftDeleteAsync(int userId);
    
    Task<Result<bool>> RestoreAsync(int userId);
    Task<Result<bool>> ChangeUsernameAsync(int userId, string newUsername);
    Task<Result<bool>> ChangeEmailAsync(int userId, string newEmail);
    Task<Result<bool>> ChangePasswordAsync(int userId, string newPassword);
}
