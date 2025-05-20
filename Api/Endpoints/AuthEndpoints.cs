using System.Net;
using Api.Dtos;
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
    
    private static IResult Login()
    {
        return Results.Ok();
    }
    
    private static IResult Logout()
    {
        return Results.Ok();
    }
    
    private static IResult Refresh()
    {
        return Results.Ok();
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
