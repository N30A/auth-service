using Api.Dtos.Users;

namespace Api.Endpoints;

public static class UsersEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/users", GetAll);
        builder.MapGet("/users/{userId:int}", Get);
        builder.MapDelete("/users/{userId:int}", Delete);
        builder.MapPatch("/users/{userId:int}/username", ChangeUsername);
        builder.MapPatch("/users/{userId:int}/email", ChangeEmail);
        builder.MapPatch("/users/{userId:int}/password", ChangePassword);
    }

    private static IResult GetAll()
    {
        return Results.Ok();
    }
    
    private static IResult Get(int userId)
    {
        return Results.Ok();
    }
    
    private static IResult Delete(int userId)
    {
        return Results.Ok();
    }

    private static IResult ChangeUsername(int userId, ChangeUsernameDto body)
    {
        return Results.Ok();
    }
    
    private static IResult ChangeEmail(int userId, ChangeEmailDto body)
    {
        return Results.Ok();
    }
    
    private static IResult ChangePassword(int userId, ChangePasswordDto body)
    {
        return Results.Ok();
    }
}
