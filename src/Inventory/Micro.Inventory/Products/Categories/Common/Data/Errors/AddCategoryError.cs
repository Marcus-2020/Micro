using FluentResults;

namespace Micro.Inventory.Products.Categories.Common.Data.Errors;

public class AddCategoryError : IExceptionalError
{
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
    public List<IError> Reasons { get; }
    public Exception Exception { get; }
    
    public AddCategoryError(Exception ex)
    {
        Message = "An error occurred when trying to insert a category";
        Metadata = new();
        Exception = ex;
        Reasons = new();
    }
}