using System.Net;
using Core.Dtos.User;
using Core.Services.Interfaces;

namespace Api.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/users", GetAll);
        builder.MapGet("/users/{userId:int}", GetById);
        builder.MapGet("/users/{email}", GetByEmail);
        builder.MapDelete("/users/{userId:int}", Delete);
        builder.MapPost("/users/{userId:int}/restore", Restore);
        builder.MapPatch("/users/{userId:int}/username", ChangeUsername);
        builder.MapPatch("/users/{userId:int}/email", ChangeEmail);
        builder.MapPatch("/users/{userId:int}/password", ChangePassword);
    }

    private static async Task<IResult> GetAll(IUserService userService)
    {   
        var result = await userService.GetAllAsync();
        var response = new ApiResponse<MultipleUsersDto>
        {
            Data = result.Data,
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        
        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        return Results.Ok(response);
    }
    
    private static async Task<IResult> GetById(int userId, IUserService userService)
    {
        var result = await userService.GetByIdAsync(userId);
        var response = new ApiResponse<UserDto>
        {
            Data = result.Data,
            Message = result.Message,
            StatusCode = result.StatusCode
        };

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        return Results.Ok(response);
    }
    
    private static async Task<IResult> GetByEmail(string email, IUserService userService)
    {
        var result = await userService.GetByEmailAsync(email);
        var response = new ApiResponse<UserDto>
        {
            Data = result.Data,
            Message = result.Message,
            StatusCode = result.StatusCode
        };

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        return Results.Ok(response);
    }
    
    private static async Task<IResult> Delete(int userId, IUserService userService)
    {
        var result = await userService.SoftDeleteAsync(userId);
        var response = new ApiNoDataResponse
        {
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        
        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        return Results.NoContent();
    }
    
    private static async Task<IResult> Restore(int userId, IUserService userService)
    {
        var result = await userService.RestoreAsync(userId);
        var response = new ApiNoDataResponse
        {
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        
        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        return Results.NoContent();
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
