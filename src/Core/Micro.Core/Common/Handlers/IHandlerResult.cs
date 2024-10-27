using System.Diagnostics;
using Serilog;

namespace Micro.Core.Common.Handlers;

public interface IHandlerResult<TRequest>
{
    TRequest Request { get; }
    ILogger Logger { get; }
    Stopwatch Stopwatch { get; }
}