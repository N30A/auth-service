using System.Net;
using Api.Dtos;
using Api.Mappers;
using Api.Services.Interfaces;

namespace Api.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/users", GetAll);
        builder.MapGet("/users/{userId:guid}", GetById);
        builder.MapGet("/users/{email}", GetByEmail);
        builder.MapDelete("/users/{userId:guid}", Delete);
        builder.MapPost("/users/{userId:guid}/restore", Restore);
    }

    private static async Task<IResult> GetAll(IUserService userService)
    {   
        var result = await userService.GetAllAsync();
        var response = new ApiResponse<MultipleUsersResponseDto>
        {
            Message = result.Message,
        };
        
        if (!result.Succeeded)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        
        response.Data = UserMapper.ToDto(result.Data!);
        return Results.Ok(response);
    }
    
    private static async Task<IResult> GetById(Guid userId, IUserService userService)
    {
        var result = await userService.GetByIdAsync(userId);
        var response = new ApiResponse<UserResponseDto>
        {
            Message = result.Message,
        };

        if (!result.Succeeded)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        
        response.Data = UserMapper.ToDto(result.Data!);
        return Results.Ok(response);
    }
    
    private static async Task<IResult> GetByEmail(string email, IUserService userService)
    {
        var result = await userService.GetByEmailAsync(email);
        var response = new ApiResponse<UserResponseDto>
        {
            Message = result.Message,
        };

        if (!result.Succeeded)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        
        response.Data = UserMapper.ToDto(result.Data!);
        return Results.Ok(response);
    }
    
    private static async Task<IResult> Delete(Guid userId, IUserService userService)
    {
        var result = await userService.SoftDeleteAsync(userId);
        var response = new ApiResponse<bool?>
        {   
            Data = null,
            Message = result.Message
        };
        
        if (!result.Succeeded)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        return Results.NoContent();
    }
    
    private static async Task<IResult> Restore(Guid userId, IUserService userService)
    {
        var result = await userService.RestoreAsync(userId);
        var response = new ApiResponse<bool?>
        {   
            Data = null,
            Message = result.Message
        };
        
        if (!result.Succeeded)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(response),
                _ => Results.InternalServerError(response)
            };
        }
        return Results.NoContent();
    }
}
