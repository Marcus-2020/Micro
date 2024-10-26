using FluentResults;

namespace Micro.Inventory.Products.Common.Data.Errors;

public class UpdateProductError : IExceptionalError
{
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
    public List<IError> Reasons { get; }
    public Exception Exception { get; }
    
    public UpdateProductError(Exception ex, string productId)
    {
        Message = $"An error occurred when trying to update the product {productId}";
        Metadata = new();
        Metadata.Add("ProductId", productId);
        Exception = ex;
        Reasons = new();
    }
}