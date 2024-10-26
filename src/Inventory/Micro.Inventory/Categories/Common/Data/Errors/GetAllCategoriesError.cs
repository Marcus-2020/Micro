using FluentResults;

namespace Micro.Inventory.Categories.Common.Data.Errors;

public class GetAllCategoriesError : IExceptionalError
{
    public string Message { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
    public List<IError> Reasons { get; init; }
    public Exception Exception { get; init; }

    public GetAllCategoriesError(Exception ex)
    {
        Message = "An error occurred when trying to recover a list of categories";
        Metadata = new();
        Exception = ex;
        Reasons = new();
    }
}