using System.Data;
using System.Data.Common;
using FluentResults;
using Npgsql;

namespace Micro.Core.Common.Data;

public class DataContext : IDataContext
{
    private readonly DatabaseOptions _options;
    
    private NpgsqlConnection? _connection;
    public DbConnection? Connection => _connection;
    public DbTransaction? Transaction => _transaction;

    private NpgsqlTransaction? _transaction;

    public DataContext(DatabaseOptions options)
    {
        if (options is null or {DefaultConnection: null} or {DefaultConnection: ""}) 
            throw new InvalidOperationException("Tried to initialize the a data context without defined database options or connection string");
        _options = options;
    }

    public bool IsConnectionOpen => _connection?.State == ConnectionState.Open;

    public async Task<Result> BeginTransactionAsync()
    {
        if (_transaction is not null)
            return Result.Fail("A transaction is already open");
        if (_connection is not null)
            return Result.Fail("A connection is already open");

        try
        {
            _connection = new NpgsqlConnection(_options.DefaultConnection);
            _transaction = await _connection.BeginTransactionAsync();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to start database connection")
                .CausedBy(ex));
        }

        return Result.Ok();
    }

    public async Task<Result> CommitAsync()
    {
        if (_connection is null)
            return Result.Fail("A connection is not open");
        if (_transaction is null)
            return Result.Fail("A transaction is not open");


        try
        {
            await _transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to start commit the current transaction")
                .CausedBy(ex));
        }
        
        return Result.Ok();
    }
    
    public async Task<Result> RollbackAsync()
    {
        if (_connection is null)
            return Result.Fail("A connection is not open");
        if (_transaction is null)
            return Result.Fail("A transaction is not open");

        try
        {
            await _transaction.RollbackAsync();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to start rollback the current transaction")
                .CausedBy(ex));
        }
        
        return Result.Ok();
    }

    public async Task<Result> FinallyAsync()
    {
        if (_connection is null)
            return Result.Fail("A connection is not open");
        if (_transaction is null)
            return Result.Fail("A transaction is not open");

        try
        {
            await _transaction.DisposeAsync();
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to start rollback the current transaction")
                .CausedBy(ex));
        }
        
        return Result.Ok();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        
        if (_transaction is not null) 
            _transaction.Dispose();
        _transaction = null;
        
        if (_connection is not null)
        {
            if (_connection.State != ConnectionState.Closed) _connection.Close();
            _connection.Dispose();
        }

        _connection = null;
    }
    
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_transaction is not null) 
            await _transaction.DisposeAsync().ConfigureAwait(false);
        _transaction = null;
        
        if (_connection is not null)
        {
            if (_connection.State != ConnectionState.Closed) await _connection.CloseAsync().ConfigureAwait(false);
            await _connection.DisposeAsync().ConfigureAwait(false);
        }

        _connection = null;
    }
}