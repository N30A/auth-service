using System.Net;
using Api.Dtos;
using Api.Mappers;
using Api.Services.Interfaces;
using Api.Validators;
using FluentValidation;

namespace Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/auth/register", Register);
        builder.MapPost("/auth/login", Login);
        builder.MapPost("/auth/logout", Logout);
        builder.MapPost("/auth/refresh", Refresh);
        builder.MapGet("/auth/validate", Validate);
    }
    
    private static async Task<IResult> Register(
        RegisterUserDto dto, 
        IAuthService authService, 
        IUserService userService, 
        IValidator<RegisterUserDto> validator)
    {
        ApiResponse<UserResponseDto?> response;
        
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            response = new ApiResponse<UserResponseDto?>
            {
                Data = null,
                Message = "Validation failed",
                Errors = validationResult.ToValidationErrorDtos()
            };
            return Results.BadRequest(response);
        }
        
        var result = await authService.RegisterAsync(dto);
        response = new ApiResponse<UserResponseDto?>
        {   
            Data = null,
            Message = result.Message,
            Errors = null
        };
        if (!result.Succeeded)
        {
            if (result.StatusCode != HttpStatusCode.Conflict)
            {
                return result.StatusCode switch
                {
                    HttpStatusCode.NotFound => Results.NotFound(response),
                    _ => Results.InternalServerError(response)
                };
            }
                
            response.Message = "Conflict";
            response.Errors = ParseConflictMessage(result.Message);
            return Results.Conflict(response);
        }
        
        response.Data = result.Data;
        return Results.Ok(response);
    }
    
    private static async Task<IResult> Login(
        LoginUserDto dto, 
        HttpContext context, 
        IAuthService authService, 
        IValidator<LoginUserDto> validator
    )
    {
        ApiResponse<AuthResponseDto?> response;
        
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            response = new ApiResponse<AuthResponseDto?>
            {
                Data = null,
                Message = "Validation failed",
                Errors = validationResult.ToValidationErrorDtos()
            };
            return Results.BadRequest(response);
        }

        var clientId = context.Request.Headers["X-Client-Id"].ToString();
        if (string.IsNullOrWhiteSpace(clientId))
        {   
            response = new ApiResponse<AuthResponseDto?>
            {
                Data = null,
                Message = "X-Client-Id is required",
                Errors = null
            };
            return Results.BadRequest(response);
        }
        
        var result = await authService.LoginAsync(dto, new AuthContextDto
        {   
            ClientId = clientId,
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers.UserAgent.ToString()
        });
        response = new ApiResponse<AuthResponseDto?>
        {   
            Data = null,
            Message = result.Message,
            Errors = null
        };
        if (!result.Succeeded)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.BadRequest => Results.BadRequest(response),
                HttpStatusCode.Unauthorized => Results.Unauthorized(),
                _ => Results.InternalServerError(response)
            };
        }
        
        response.Data = AuthMapper.ToDto(result.Data!);
        context.Response.Cookies.Append("refreshToken", result.Data!.RefreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Expires = result.Data!.RefreshToken.ExpiresAt,
            SameSite = SameSiteMode.Strict,
            Secure = true
        });
        return Results.Ok(response);
    }
    
    private static async Task<IResult> Logout(HttpContext context, IAuthService authService)
    {
        string? refreshToken = context.Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Results.Unauthorized();
        }
        
        var result = await authService.LogoutAsync(refreshToken);
        if (!result.Succeeded)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.Unauthorized => Results.Unauthorized(),
                _ => Results.InternalServerError(result)
            };
        }
        
        context.Response.Cookies.Delete("refreshToken");
        return Results.NoContent();
    }
    
    private static async Task<IResult> Refresh(HttpContext context, IAuthService authService)
    {
        var clientId = context.Request.Headers["X-Client-Id"].ToString();
        if (string.IsNullOrWhiteSpace(clientId))
        {   
            return Results.BadRequest(new ApiResponse<AuthResponseDto?>
            {
                Data = null,
                Message = "X-Client-Id is required",
                Errors = null
            });
        }
        
        string? refreshToken = context.Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Results.Unauthorized();
        }
        
        var result = await authService.RefreshAsync(refreshToken, new AuthContextDto
        {   
            ClientId = clientId,
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers.UserAgent.ToString()
        });
        var response = new ApiResponse<AuthResponseDto?>
        {
            Data = null,
            Message = result.Message,
            Errors = null
        };
        if (!result.Succeeded)
        {
            return result.StatusCode switch
            {   
                HttpStatusCode.BadRequest => Results.BadRequest(response),
                HttpStatusCode.Unauthorized => Results.Unauthorized(),
                _ => Results.InternalServerError(response)
            };
        }
        response.Data = AuthMapper.ToDto(result.Data!);
        return Results.Ok(response);
    }
    
    private static IResult Validate()
    {
        return Results.Ok();
    }
    
    private static List<ValidationErrorDto> ParseConflictMessage(string message)
    {
        var errors = new List<ValidationErrorDto>();
        
        if(message.Contains("username"))
        {
            errors.Add(new ValidationErrorDto { Field = "username", Message = "Username is already taken" });
        }
        
        if(message.Contains("email"))
        {
            errors.Add(new ValidationErrorDto { Field = "email", Message = "Email is already taken" });
        }
        return errors;
    }
}
