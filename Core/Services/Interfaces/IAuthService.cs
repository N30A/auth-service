using Core.Dtos.Auth;
using Core.Dtos.User;

namespace Core.Services.Interfaces;

public interface IAuthService
{
    Task<Result<UserDto?>> RegisterAsync(RegisterDto registerDto);
}