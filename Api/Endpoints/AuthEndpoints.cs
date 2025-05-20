using Api.Dtos;

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

    private static IResult Register(RegisterUserDto body)
    {
        return Results.Ok();
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
