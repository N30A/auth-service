using Api.Dtos;

namespace Api.Services.Interfaces;

public interface IAuthService
{
    Task<Result<UserResponseDto?>> RegisterAsync(RegisterUserDto registerDto);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangeUserPasswordDto changePasswordDto);
}
