namespace Micro.Core.Common.Handlers;

public interface IHandler<in TRequest, TResponse>
{
    public Task<TResponse> HandleAsync(TRequest request);
}