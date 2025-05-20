namespace Api.Models;

public class UpdateUserModel
{   
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}
