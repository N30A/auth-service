using Data.Models;

namespace Core.Dtos.User;

public class MultipleUsersDto
{
    public IEnumerable<UserDto> Users { get; } = [];
    public int Count => Users.Count();
    
    private MultipleUsersDto() {}

    public MultipleUsersDto(IEnumerable<UserModel> users)
    {
        Users = users.Select(user => new UserDto
        {
            Id = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            DeletedAt = user.DeletedAt,
        }).ToList();
    }
    
    public static MultipleUsersDto Empty => new(); 
}
