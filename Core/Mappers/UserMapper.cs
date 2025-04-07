using Core.Dtos.User;
using Data.Models;

namespace Core.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(UserModel user) => new()
    {
        Id = user.UserId,
        Username = user.Username,
        Email = user.Email,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        DeletedAt = user.DeletedAt
    };

    public static UserModel ToModel(UserDto user) => new()
    {
        UserId = user.Id,
        Username = user.Username,
        Email = user.Email,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        DeletedAt = user.DeletedAt
    };

    public static MultipleUsersDto ToMultipleDto(IEnumerable<UserModel> users) => new()
    {
        Users = users.Select(ToDto)
    };
}
