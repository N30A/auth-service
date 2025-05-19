using Core.Dtos.Auth;
using Core.Dtos.User;
using Core.Services.Interfaces;

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

    private static async Task<IResult> Register(RegisterDto body, IAuthService authService)
    {
        var result = await authService.RegisterAsync(body);
        var response = new ApiResponse<UserDto>
        {   
            Data = result.Data,
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        
        if (!result.IsSuccess)
        {
            Results.InternalServerError(response);
        }

        return Results.Created($"/users/{response.Data?.Id}", response);
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
}
