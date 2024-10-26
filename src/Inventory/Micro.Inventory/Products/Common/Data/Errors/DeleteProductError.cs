using FluentResults;

namespace Micro.Inventory.Products.Common.Data.Errors;

public class DeleteProductError : IExceptionalError
{
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
    public List<IError> Reasons { get; }
    public Exception Exception { get; }
    
    public DeleteProductError(Exception ex, string productId)
    {
        Message = $"An error occurred when trying to delete the product {productId}";
        Metadata = new();
        Metadata.Add("ProductId", productId);
        Exception = ex;
        Reasons = new();
    }
}