using FluentResults;
using Micro.Core.Common.Responses;

namespace Micro.Inventory.Common.Errors;

public class ValidationError : IError
{
    public int StatusCode { get; }
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
    public List<IError> Reasons { get; }
    
    public List<ResponseError> ValidationErrors { get; }
    
    public ValidationError(string message, List<ResponseError> errors, Dictionary<string, object>? metadata = null, List<IError>? reasons = null)
    {
        StatusCode = 422;
        Message = message;
        ValidationErrors = errors;
        Metadata = metadata ?? new();
        Reasons = reasons ?? new();
    }
}