namespace Micro.Core.Common.Data;

public interface IDataContextFactory
{
    IDataContext CreateDataContext();
}