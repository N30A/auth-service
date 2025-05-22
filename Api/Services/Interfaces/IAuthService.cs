using Api.Dtos;
using Api.Models;

namespace Api.Services.Interfaces;

public interface IAuthService
{
    Task<Result<UserResponseDto?>> RegisterAsync(RegisterUserDto registerDto);
    Task<Result<AuthResultModel?>> LoginAsync(LoginUserDto loginDto, AuthContextDto context);
    Task<Result<bool>> LogoutAsync(string refreshToken);
    Task<Result<AuthResultModel?>> RefreshAsync(string refreshToken, AuthContextDto context);
    Result<string?> Validate(string accessToken, string clientId, IConfiguration configuration);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangeUserPasswordDto changePasswordDto);
}
