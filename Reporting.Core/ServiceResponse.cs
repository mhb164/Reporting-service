namespace Tizpusoft.Reporting;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }

    public ServiceResponse(string message, bool success = false, T? data = default(T?))
    {
        Message = message;
        Data = data;
        Success = success;
    }

    public static ServiceResponse<T> Failed(string message)
       => new ServiceResponse<T>(message, false);

    public static ServiceResponse<T> Ok(T data, string message = "OK")
       => new ServiceResponse<T>(message, true, data);
}

public class ServiceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }

    private ServiceResponse(string message, bool success = false)
    {
        Message = message;
        Success = success;
    }

    public static ServiceResponse Failed(string message)
        => new ServiceResponse(message, false);

    public static ServiceResponse Ok(string message)
       => new ServiceResponse(message, true);
}