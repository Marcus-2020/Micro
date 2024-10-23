using System.Data.Common;
using FluentResults;

namespace Micro.Core.Common.Data;

public interface IDataContext : IAsyncDisposable, IDisposable
{
    DbConnection? Connection { get; }
    DbTransaction? Transaction { get; }
    bool IsConnectionOpen { get; }
    
    Task<Result> BeginTransactionAsync();
    Task<Result> CommitAsync();
    Task<Result> RollbackAsync();
    Task<Result> FinallyAsync();
}