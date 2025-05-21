namespace Api.Models;

public class AuthResultModel
{
    public string AccessToken { get; set; }
    public RefreshTokenModel RefreshToken { get; set; }
}
