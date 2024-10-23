using FluentResults;

namespace Micro.Inventory.Products.Common.Data.Errors;

public class AddProductError : IExceptionalError
{
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
    public List<IError> Reasons { get; }
    public Exception Exception { get; }
    
    public AddProductError(Exception ex)
    {
        Message = "An error occurred when trying to insert a product";
        Metadata = new();
        Exception = ex;
        Reasons = new();
    }
}