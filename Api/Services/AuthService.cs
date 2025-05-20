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
        registerDto.Email = registerDto.Email.ToLower().Trim();
        registerDto.Username = registerDto.Username.Trim();
        
        var conflicts = new List<string>();
        
        var emailResult = await _userService.GetByEmailAsync(registerDto.Email);
        if (emailResult.Succeeded)
        {
            conflicts.Add("email");
        }
        else if (emailResult.StatusCode != HttpStatusCode.NotFound)
        {
            return Result<UserResponseDto?>.Failure(emailResult.Message, null, emailResult.StatusCode);
        }
        
        var usernameResult = await _userService.GetByUsernameAsync(registerDto.Username);
        if (usernameResult.Succeeded)
        {
            conflicts.Add("username");
        }
        else if (usernameResult.StatusCode != HttpStatusCode.NotFound)
        {
            return Result<UserResponseDto?>.Failure(usernameResult.Message, null, usernameResult.StatusCode);
        }

        if (conflicts.Count > 0)
        {
            string message = string.Join(" ", conflicts);
            return Result<UserResponseDto?>.Failure(message, null, HttpStatusCode.Conflict);
        }
        
        var newUser = UserMapper.ToModel(registerDto);
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
