using System.Net;
using Api.Dtos;
using Api.Mappers;
using Api.PasswordHashers;
using Api.Services.Interfaces;

namespace Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    
    public AuthService(IUserService userService, IPasswordHasher passwordHasher)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserResponseDto?>> RegisterAsync(RegisterUserDto registerDto)
    {
        var newUser = UserMapper.ToModel(registerDto);
        
        var existsResult = await _userService.GetByEmailAsync(newUser.Email);
        if (existsResult.Succeeded)
        {   
            if (existsResult.Data!.Username == newUser.Username)
            {
                return Result<UserResponseDto?>.Failure("Username already used", null, HttpStatusCode.Conflict);
            }
            
            return Result<UserResponseDto?>.Failure("Email already used", null, HttpStatusCode.Conflict);
        }
        
        newUser.PasswordHash = _passwordHasher.HashPassword(registerDto.Password);
        
        var result = await _userService.AddAsync(newUser);
        if (!result.Succeeded)
        {
            return Result<UserResponseDto?>.Failure(result.Message);
        }
        
        return Result<UserResponseDto?>.Success("User registered successfully", UserMapper.ToDto(result.Data!));
    }
    
    public async Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangeUserPasswordDto changePasswordDto)
    {
        throw new NotImplementedException();
    }
}
