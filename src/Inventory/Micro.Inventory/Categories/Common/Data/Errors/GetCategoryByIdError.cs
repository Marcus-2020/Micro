using FluentResults;

namespace Micro.Inventory.Categories.Common.Data.Errors;

public class GetCategoryByIdError : IExceptionalError
{
    public string Message { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
    public List<IError> Reasons { get; init; }
    public Exception Exception { get; init; }

    public GetCategoryByIdError(Exception ex, string categoryId)
    {
        Message = $"An error occurred when trying to recover a category by it's ID: {categoryId}";
        Metadata = new();
        Metadata.Add("CategoryId", categoryId);
        Exception = ex;
        Reasons = new();
    }
}