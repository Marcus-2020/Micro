using FluentResults;
using FluentValidation;
using Micro.Core.Common.Errors;
using Micro.Core.Common.Responses;
using Serilog;

namespace Micro.Core.Common.Handlers;

public abstract class Handler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    protected abstract IValidator<TRequest> _validator { get; }
    
    public abstract Task<TResponse> HandleAsync(TRequest request);

    protected ILogger GetMethodLogger(ILogger parentLogger, string methodName)
    {
        return parentLogger.ForContext("method", nameof(methodName));
    }
    
    protected async Task<Result<T>> ValidateRequestAsync<T>(T result) where T : IHandlerResult<TRequest>
    {
        var logger = GetMethodLogger(result.Logger, nameof(ValidateRequestAsync));
        
        var results = await _validator.ValidateAsync(result.Request);
        if (results.IsValid) return result;
        
        logger.ForContext("validationErrors", results.Errors.Select(x=>new{x.PropertyName, x.ErrorMessage}), true)
            .Warning("Invalid request at {Timestamp} after {ElapsedMilliseconds}ms", DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
        
        return Result.Fail(new BadRequestError(
            "Invalid request",
            results.Errors
                .Select(x => new ResponseError(x.PropertyName, x.ErrorMessage))
                .ToList()));
    }
}