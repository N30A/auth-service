using System.Net;

namespace Core;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public T? Data { get; private set; }
    public HttpStatusCode StatusCode { get; private set; }

    private Result(bool isSuccess, string message, T? data, HttpStatusCode statusCode)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }

    public static Result<T> Success(string message, T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new Result<T>(true, message, data, statusCode);
    }

    public static Result<T> Failure(string message, T? data = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        return new Result<T>(false, message, data, statusCode);
    }
}
