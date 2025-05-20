using System.Net;

namespace Api.Services;

public class Result<T>
{
    public bool Succeeded { get; private set; }
    public T? Data { get; private set; }
    public string Message { get; private set; }
    public HttpStatusCode StatusCode { get; private set; }

    private Result(bool succeeded, T? data, string message, HttpStatusCode statusCode)
    {
        Succeeded = succeeded;
        Data = data;
        Message = message;
        StatusCode = statusCode;
    }

    public static Result<T> Success(string message, T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new Result<T>(true, data, message, statusCode);
    }

    public static Result<T> Failure(
        string message = "Internal server error",
        T? data = default, 
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError
    )
    {
        return new Result<T>(false, data, message , statusCode);
    }
    
    public static Result<T> NotFound(string message)
    {
        return new Result<T>(
            false,
            default,
            message,
            HttpStatusCode.NotFound
        );
    }
}
