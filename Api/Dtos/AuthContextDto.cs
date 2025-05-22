namespace Api.Dtos;

public class AuthContextDto
{
    public string ClientId { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}
