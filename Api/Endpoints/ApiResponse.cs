using Api.Dtos;

namespace Api.Endpoints;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ValidationErrorDto>? Errors { get; set; }
}
