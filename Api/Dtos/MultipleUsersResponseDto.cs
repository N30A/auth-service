namespace Api.Dtos;

public class MultipleUsersResponseDto
{
    public IEnumerable<UserResponseDto> Users { get; set; } = [];
    public int Count => Users.Count();
    
    public static MultipleUsersResponseDto Empty => new(); 
}
