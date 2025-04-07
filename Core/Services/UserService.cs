using System.Net;
using Core.Dtos.User;
using Core.Services.Interfaces;
using Data.Repositories.Interfaces;

namespace Core.Services;

public class UserService : IUserService
{   
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<MultipleUsersDto>> GetAllAsync()
    {
        var dbResult = await _userRepository.GetAllAsync();
        var data = dbResult.Data?.ToList() ?? [];
        
        if (!dbResult.IsSuccess)
        {
            return Result<MultipleUsersDto>.Failure("Unknown error", new MultipleUsersDto(data));
        }
        
        if (data.Count == 0)
        {
            return Result<MultipleUsersDto>.Failure("No users found", MultipleUsersDto.Empty, HttpStatusCode.NotFound);
        }
        
        return Result<MultipleUsersDto>.Success("Success", new MultipleUsersDto(data));
    }

    public async Task<Result<UserDto?>> GetByIdAsync(int userId)
    {
        var dbResult = await _userRepository.GetByIdAsync(userId);
        
        if (!dbResult.IsSuccess)
        {
            return Result<UserDto?>.Failure("Unknown error");
        }

        if (dbResult.Data == null)
        {
            return Result<UserDto?>.Failure("No user found", statusCode: HttpStatusCode.NotFound);
        }

        return Result<UserDto?>.Success("Success");
    }

    public async Task<Result<UserDto?>> GetByEmailAsync(string email)
    {
        var dbResult = await _userRepository.GetByEmailAsync(email);
        
        if (!dbResult.IsSuccess)
        {
            return Result<UserDto?>.Failure("Unknown error");
        }

        if (dbResult.Data == null)
        {
            return Result<UserDto?>.Failure("No user found", statusCode: HttpStatusCode.NotFound);
        }

        return Result<UserDto?>.Success("Success");
    }

    public async Task<Result<int?>> AddAsync(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> SoftDeleteAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> ChangeUsernameAsync(int userId, string newUsername)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> ChangeEmailAsync(int userId, string newEmail)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> ChangePasswordAsync(int userId, string newPassword)
    {
        throw new NotImplementedException();
    }
}
