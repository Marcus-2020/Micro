using FluentResults;

namespace Micro.Core.Common.Errors;

public class InternalServerError : IError
{
    public int StatusCode { get; }
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
    public List<IError> Reasons { get; }
    public Exception? Exception { get; }
    
    public InternalServerError(string message, Exception? ex, Dictionary<string, object>? metadata = null, List<IError>? reasons = null)
    {
        StatusCode = 500;
        Exception = ex;
        Message = message;
        Metadata = metadata ?? new();
        Reasons = reasons ?? new();
    }
}