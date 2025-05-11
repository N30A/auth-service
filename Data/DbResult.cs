namespace Data;

public class DbResult<T>
{
    public bool Succeeded { get; private set; }
    public T? Data { get; private set; }
    public DbErrorType? ErrorType { get; private set; }

    private DbResult(bool succeeded, T? data, DbErrorType? errorType)
    {
        Succeeded = succeeded;
        Data = data;
        ErrorType = errorType;
    }

    public static DbResult<T> Success(T data)
    {
        return new DbResult<T>(true, data, null);
    }

    public static DbResult<T> Failure(T? data = default, DbErrorType? errorType = DbErrorType.Unexpected)
    {
        return new DbResult<T>(false, data, errorType);
    }
}
