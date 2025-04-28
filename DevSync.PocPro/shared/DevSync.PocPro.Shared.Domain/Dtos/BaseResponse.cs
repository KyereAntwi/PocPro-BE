namespace DevSync.PocPro.Shared.Domain.Dtos;

public class BaseResponse<T> where T : notnull
{
    public BaseResponse(string message, bool success)
    {
        Message = message;
        Success = success;
    }
    
    public T? Data { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
    public IEnumerable<string> Errors { get; set; }
}