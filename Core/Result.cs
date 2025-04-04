namespace Core;

public class Result<T>
{
    public bool Status { get; private set; }
    public string Message { get; private set; }
    public T? Data { get; private set; }
    public int StatusCode { get; private set; }

    private Result(bool status, string message, T? data, int statusCode)
    {
        Status = status;
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T? data = default, int statusCode = 200)
    {
        return Success(string.Empty, data, statusCode);
    }

    public static Result<T> Success(string message, T? data = default, int statusCode = 200)
    {
        return new Result<T>(true, message, data, statusCode);
    }

    public static Result<T> Failure(string message, T? data = default, int statusCode = 500)
    {
        return new Result<T>(false, message, data, statusCode);
    }

    public static Result<T> Failure(Exception ex, int statusCode = 500)
    {
        return Failure(ex.Message, default(T), statusCode);
    }
}
