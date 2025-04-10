using System.Net;
using Core.Dtos.User;
using Core.Mappers;
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
            return Result<MultipleUsersDto>.Failure("Unknown error", MultipleUsersDto.Empty);
        }
        
        return data.Count switch
        {
            0 => Result<MultipleUsersDto>.Failure("No users found", MultipleUsersDto.Empty, HttpStatusCode.NotFound),
            _ => Result<MultipleUsersDto>.Success("Success", UserMapper.ToMultipleDto(data))
        };
    }

    public async Task<Result<UserDto?>> GetByIdAsync(int userId)
    {
        var dbResult = await _userRepository.GetByIdAsync(userId);
        
        if (!dbResult.IsSuccess)
        {
            return Result<UserDto?>.Failure("Unknown error");
        }

        return dbResult.Data switch
        {
            null => Result<UserDto?>.Failure("No user found", statusCode: HttpStatusCode.NotFound),
            _ => Result<UserDto?>.Success("Success", UserMapper.ToDto(dbResult.Data))
        };
    }

    public async Task<Result<UserDto?>> GetByEmailAsync(string email)
    {
        var dbResult = await _userRepository.GetByEmailAsync(email);
        
        if (!dbResult.IsSuccess)
        {
            return Result<UserDto?>.Failure("Unknown error");
        }

        return dbResult.Data switch
        {   
            null => Result<UserDto?>.Failure("No user found", statusCode: HttpStatusCode.NotFound),
            _ => Result<UserDto?>.Success("Success", UserMapper.ToDto(dbResult.Data))
        };
    }
    
    public async Task<Result<bool>> SoftDeleteAsync(int userId)
    {
        var result = await _userRepository.SoftDeleteAsync(userId);

        if (!result.IsSuccess)
        {
            return Result<bool>.Failure("Unknown error");
        }
        
        return result.Data switch
        {   
            true => Result<bool>.Success(string.Empty, result.Data),
            false => Result<bool>.Failure("No user found", statusCode: HttpStatusCode.NotFound)
        };
    }
    
    public async Task<Result<bool>> RestoreAsync(int userId)
    {
        var result = await _userRepository.RestoreAsync(userId);
        
        if (!result.IsSuccess)
        {
            return Result<bool>.Failure("Unknown error");
        }
        
        return result.Data switch
        {   
            true => Result<bool>.Success(string.Empty, result.Data),
            false => Result<bool>.Failure("No user found", statusCode: HttpStatusCode.NotFound)
        };
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
