using FluentResults;

namespace Micro.Inventory.Products.Common.Data.Errors;

public class GetProductByIdError : IExceptionalError
{
    public string Message { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
    public List<IError> Reasons { get; init; }
    public Exception Exception { get; init; }

    public GetProductByIdError(Exception ex, string productId)
    {
        Message = $"An error occurred when trying to recover a product by it's ID: {productId}";
        Metadata = new();
        Metadata.Add("ProductId", productId);
        Exception = ex;
        Reasons = new();
    }
}