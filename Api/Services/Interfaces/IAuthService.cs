using Api.Dtos;
using Api.Models;

namespace Api.Services.Interfaces;

public interface IAuthService
{
    Task<Result<UserResponseDto?>> RegisterAsync(RegisterUserDto registerDto);
    Task<Result<AuthResultModel?>> LoginAsync(LoginUserDto loginDto, LoginContextDto context);
    Task<Result<bool>> LogoutAsync(string refreshToken);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangeUserPasswordDto changePasswordDto);
}
