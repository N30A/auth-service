using Api.Dtos;
using Api.Models;

namespace Api.Mappers;

public static class UserMapper
{
    public static AddUserModel ToModel(RegisterUserDto registerUserDto) => new()
    {
        Username = registerUserDto.Username,
        Email = registerUserDto.Email,
    };

    public static UpdateUserModel ToModel(UpdateUserDto updateUserDto, Guid userId) => new()
    {   
        UserId = userId,
        Username = updateUserDto.Username,
        Email = updateUserDto.Email
    };
    
    public static UserResponseDto ToDto(UserModel userModel) => new()
    {
        Id = userModel.UserId,
        Username = userModel.Username,
        Email = userModel.Email,
        CreatedAt = userModel.CreatedAt,
        UpdatedAt = userModel.UpdatedAt,
        DeletedAt = userModel.DeletedAt
    };

    public static MultipleUsersResponseDto ToDto(IEnumerable<UserModel> users) => new()
    {
        Users = users.Select(ToDto)
    };
}
