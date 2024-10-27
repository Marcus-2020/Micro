using Serilog;

namespace Micro.Core.Common.Handlers;

public abstract class Handler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    public abstract Task<TResponse> HandleAsync(TRequest request);

    protected ILogger GetMethodLogger(ILogger parentLogger, string methodName)
    {
        return parentLogger.ForContext("method", nameof(HandleAsync));
    }
}