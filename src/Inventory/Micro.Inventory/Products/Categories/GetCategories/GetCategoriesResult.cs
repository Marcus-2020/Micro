using System.Diagnostics;
using Micro.Core.Common.Data;
using Micro.Core.Common.Handlers;
using Micro.Inventory.Contracts.Products.Categories.GetCategories;
using Micro.Inventory.Products.Common.Entities;
using Serilog;

namespace Micro.Inventory.Products.Categories.GetCategories;

internal record struct GetCategoriesResult(
    ILogger Logger,
    IDataContext DataContext,
    GetCategoriesRequest Request,
    Stopwatch Stopwatch) : IHandlerResult<GetCategoriesRequest>
{
    public int Count { get; set; }
    
    public List<ProductCategory> Categories { get; set; } = new();

    public List<GetCategoriesResponseItem> MapCategoriesToResponseItems()
    {
        return Categories
            .Select(x => new GetCategoriesResponseItem(x.Id, x.Name, x.Description, x.IsActive))
            .ToList();
    }
}