using System.Diagnostics;
using FluentResults;
using Micro.Core.Common.Data;
using Micro.Core.Common.Handlers;
using Micro.Inventory.Contracts.Products.Categories.CreateCategory;
using Micro.Inventory.Products.Common.Entities;
using Serilog;

namespace Micro.Inventory.Products.Categories.CreateCategory;

internal record struct CreateCategoryResult(
    ILogger Logger,
    IDataContext DataContext,
    CreateCategoryRequest Request,
    Stopwatch Stopwatch) : IHandlerResult<CreateCategoryRequest>
{
    public ProductCategory? Category { get; private set; }

    public Result SetCategory()
    {
        if (Category is not null) return Result.Fail("Category was already set");
        Category = new ProductCategory(Guid.Empty, Request.Name, Request.Description, Request.IsActive);
        return Result.Ok();
    }
};