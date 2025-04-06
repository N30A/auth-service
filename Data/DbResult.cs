namespace Data;

public class DbResult<T>
{
    public bool Status { get; private set; }
    public T? Data { get; private set; }

    private DbResult(bool status, T? data)
    {
        Status = status;
        Data = data;
    }
    
    public static DbResult<T> Success(T data)
    {
        return new DbResult<T>(true, data);
    }

    public static DbResult<T> Failure(T? data = default)
    {
        return new DbResult<T>(false, data);
    }
}
