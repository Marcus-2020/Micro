﻿using FluentResults;

namespace Micro.Inventory.Products.Categories.Common.Data.Errors;

public class UpdateCategoryError : IExceptionalError
{
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
    public List<IError> Reasons { get; }
    public Exception Exception { get; }
    
    public UpdateCategoryError(Exception ex, string categoryId)
    {
        Message = $"An error occurred when trying to update the category {categoryId}";
        Metadata = new();
        Metadata.Add("CategoryId", categoryId);
        Exception = ex;
        Reasons = new();
    }
}