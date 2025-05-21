using Api.Dtos;
using Api.Models;

namespace Api.Mappers;

public static class AuthMapper
{
    public static AuthResponseDto ToDto(AuthResultModel result) => new()
    {
        AccessToken = result.AccessToken,
    };
}