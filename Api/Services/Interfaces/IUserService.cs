using Api.Models;

namespace Api.Services.Interfaces;

public interface IUserService
{
    Task<Result<IEnumerable<UserModel>>> GetAllAsync();
    Task<Result<UserModel?>> GetByIdAsync(Guid userId);
    Task<Result<UserModel?>> GetByEmailAsync(string email);
    Task<Result<UserModel?>> GetByUsernameAsync(string username);
    Task<Result<UserModel?>> AddAsync(AddUserModel newUser);
    Task<Result<bool>> SoftDeleteAsync(Guid userId);
    Task<Result<bool>> RestoreAsync(Guid userId);
    Task<Result<UserModel?>> UpdateAsync(Guid userId, UpdateUserModel userToUpdate);
}
