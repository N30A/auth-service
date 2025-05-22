using System.Data;
using Dapper;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.Dtos;
using Api.Mappers;
using Api.Models;
using Api.PasswordHashers;
using Api.Services.Interfaces;
using Api.Services.Queries;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class AuthService : IAuthService
{   
    private readonly IDbConnection _connection;
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    
    public AuthService(IDbConnection connection, IUserService userService, IPasswordHasher passwordHasher, IConfiguration configuration)
    {   
        _connection = connection;
        _userService = userService;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
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

    public async Task<Result<AuthResultModel?>> LoginAsync(LoginUserDto loginDto, LoginContextDto context)
    {
        var userResult = await ValidateUserAsync(loginDto.Email, loginDto.Password);
        if (!userResult.Succeeded)
        {
            return Result<AuthResultModel?>.Failure(userResult.Message, null, userResult.StatusCode);
        }
        
        string? accessToken = GenerateAccessToken(userResult.Data!.UserId, context.ClientId);
        if (accessToken == null)
        {
            return Result<AuthResultModel?>.Failure(
                "Invalid clientId/audience",
                null,
                HttpStatusCode.Unauthorized
            );
        }
        
        try
        {
            var refreshToken = GenerateRefreshToken(userResult.Data!.UserId, context);
            await _connection.ExecuteAsync(RefreshTokenQueries.Add, refreshToken);
            return Result<AuthResultModel?>.Success("Successfully logged in", new AuthResultModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
            
        }
        catch (Exception)
        {
            return Result<AuthResultModel?>.Failure();
        }
    }

    public async Task<Result<bool>> LogoutAsync(string refreshToken)
    {
        var dbToken = await GetRefreshTokenByHash(HashToken(refreshToken));
        if (dbToken == null)
        {
            return Result<bool>.Failure(
                "Refresh token not found",
                false,
                HttpStatusCode.Unauthorized
            );
        }
        
        if (dbToken.RevokedAt != null || dbToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result<bool>.Failure(
                "Refresh token is no longer valid", 
                false, 
                HttpStatusCode.Unauthorized
            );
        }
        
        try
        {
            await _connection.ExecuteAsync(
                RefreshTokenQueries.SetRevoked, 
                new { dbToken.RefreshTokenId}
            );
            return Result<bool>.Success("Refresh token successfully revoked", true);
        }
        catch (Exception)
        {
            return Result<bool>.Failure();
        }
    }

    private async Task<RefreshTokenModel?> GetRefreshTokenByHash(string tokenHash)
    {
        try
        {
            var dbToken = await _connection.QuerySingleOrDefaultAsync<RefreshTokenModel?>(
                RefreshTokenQueries.GetByHash, new { TokenHash = tokenHash }
            );
            return dbToken ?? null;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<Result<UserModel?>> ValidateUserAsync(string email, string password)
    {
        var userResult = await _userService.GetByEmailAsync(email);
        if (!userResult.Succeeded)
        {
            return userResult.StatusCode switch
            {
                HttpStatusCode.NotFound => Result<UserModel?>.Failure(
                    "Invalid username or password", 
                    null, 
                    HttpStatusCode.BadRequest
                ),
                _ => Result<UserModel?>.Failure()
            };
        }
        
        if (!_passwordHasher.Verify(userResult.Data!.PasswordHash, password))
        {
            return Result<UserModel?>.Failure(
                "Invalid username or password",
                null,
                HttpStatusCode.Unauthorized
            );
        }
        return Result<UserModel?>.Success("User validated successfully", userResult.Data!);
    }
    
    public async Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangeUserPasswordDto changePasswordDto)
    {
        throw new NotImplementedException();
    }
    
    private string? GenerateAccessToken(Guid userId, string clientId)
    {   
        string[] allowedAudiences = _configuration
            .GetValue<string>("JWT_AUDIENCE")!
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (!allowedAudiences.Contains(clientId))
        {
            return null;
        }
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        };

        var jwtIssuer = _configuration.GetValue<string>("JWT_ISSUER");
        var jwtKey = _configuration.GetValue<string>("JWT_KEY");
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
            SecurityAlgorithms.HmacSha256
        );
        
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: clientId,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static RefreshTokenModel GenerateRefreshToken(Guid userId, LoginContextDto context)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        string token = Convert.ToBase64String(randomBytes);
        return new RefreshTokenModel
        {   
            Token = token,
            TokenHash = HashToken(token),
            UserId = userId,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = null,
            IsUsed = false,
            UserAgent = context.UserAgent,
            IpAddress = context.IpAddress
        };
    }
    
    private static string HashToken(string token)
    {
        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        byte[] hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
