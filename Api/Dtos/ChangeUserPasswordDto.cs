namespace Api.Dtos;

public class ChangeUserPasswordDto
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}
