namespace Core.Dtos.User;

public class MultipleUsersDto
{
    public IEnumerable<UserDto> Users { get; set; } = [];
    public int Count => Users.Count();
    
    public static MultipleUsersDto Empty => new(); 
}
