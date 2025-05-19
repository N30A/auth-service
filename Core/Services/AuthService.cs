using Core.Dtos.Auth;
using Core.Dtos.User;
using Core.PasswordHasher;
using Core.Services.Interfaces;

namespace Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    
    public AuthService(IUserService userService, IPasswordHasher passwordHasher)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto?>> RegisterAsync(RegisterDto registerDto)
    {
        string hashedPassword = _passwordHasher.HashPassword(registerDto.Password);

        var newUser = new AddUserDto
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = hashedPassword
        };

        var existsResult = await _userService.GetByEmailAsync(newUser.Email);
        if (existsResult.IsSuccess)
        {
            return Result<UserDto?>.Failure("Email already used");
        }
        
        var result = await _userService.AddAsync(newUser);
        if (!result.IsSuccess)
        {
            return Result<UserDto?>.Failure(result.Message);
        }
        
        return Result<UserDto?>.Success("Success", result.Data);
    }
}