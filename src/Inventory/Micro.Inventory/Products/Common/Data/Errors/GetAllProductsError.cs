using FluentResults;

namespace Micro.Inventory.Products.Common.Data.Errors;

public class GetAllProductsError : IExceptionalError
{
    public string Message { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
    public List<IError> Reasons { get; init; }
    public Exception Exception { get; init; }

    public GetAllProductsError(Exception ex)
    {
        Message = "An error occurred when trying to recover a list of products";
        Metadata = new();
        Exception = ex;
        Reasons = new();
    }
}