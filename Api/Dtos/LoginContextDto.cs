namespace Api.Dtos;

public class LoginContextDto
{
    public string ClientId { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}
