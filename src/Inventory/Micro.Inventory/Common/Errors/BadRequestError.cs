using FluentResults;
using Micro.Core.Common.Responses;

namespace Micro.Inventory.Common.Errors;

public class BadRequestError : IError
{
    public int StatusCode { get; }
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
    public List<IError> Reasons { get; }
    
    public List<ResponseError> ValidationErrors { get; }
    
    public BadRequestError(string message, List<ResponseError>? errors = null, Dictionary<string, object>? metadata = null, List<IError>? reasons = null)
    {
        StatusCode = 400;
        Message = message;
        ValidationErrors = errors ?? new();
        Metadata = metadata ?? new();
        Reasons = reasons ?? new();
    }
}