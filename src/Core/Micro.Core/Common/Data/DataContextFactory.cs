using Microsoft.Extensions.Options;

namespace Micro.Core.Common.Data;

public class DataContextFactory : IDataContextFactory
{
    private readonly DatabaseOptions _options;
    
    public DataContextFactory(IOptions<DatabaseOptions> options)
    {
        if (options.Value is null or {DefaultConnection: null} or {DefaultConnection: ""}) 
            throw new InvalidOperationException("Tried to initialize the a data context factory without defined database options or connection string");
        _options = options.Value;
    }
    
    public IDataContext CreateDataContext()
    {
        return new DataContext(_options);
    }
}