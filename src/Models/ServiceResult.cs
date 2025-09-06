namespace EasierDocuware.Models
{
  public class ServiceResult<T>
  {
    public bool Success { get; private set; }
    public string? Message { get; private set; }
    public T? Data { get; private set; }

    private ServiceResult(bool success, T? data, string? message)
    {
      Success = success;
      Data = data;
      Message = message;
    }

    public static ServiceResult<T> Ok(T data) => new(true, data, null);
    public static ServiceResult<T> Fail(string message) => new(false, default, message);
  }
}
