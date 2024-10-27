using FluentResults;

namespace Micro.Inventory.Common.Errors;

public class BadRequestError : IError
{
    public int StatusCode { get; }
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
    public List<IError> Reasons { get; }
    
    public BadRequestError(string message, Dictionary<string, object>? metadata = null, List<IError>? reasons = null)
    {
        StatusCode = 400;
        Message = message;
        Metadata = metadata ?? new();
        Reasons = reasons ?? new();
    }
}