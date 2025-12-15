using System.ComponentModel.DataAnnotations;

namespace PartitionTableFullStack.API.Common;

public enum APIResponseStatus
{
    Success,
    Error,
    ValidationError,
    NotFound,
    Unauthorized
}

public class ServiceResponse<T>
{
    public T? Result { get; set; }
    public APIResponseStatus ApiResponseStatus { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<object>? ValidationResults { get; set; }
}
