using System.Diagnostics;
using FluentResults;
using Micro.Core.Common.Data;
using Micro.Inventory.Contracts.Products.CreateProduct;
using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.CreateProduct.Mappers;
using Serilog;

namespace Micro.Inventory.Products.CreateProduct;

internal record struct CreateProductResult(
    ILogger Logger,
    IDataContext DataContext,
    CreateProductRequest Request,
    Stopwatch Stopwatch)
{
    public Product? Product { get; private set; }
    
    public Result SetProduct()
    {
        if (Product is not null) return Result.Fail("Product was already set");
        Product = Request.ToProduct();
        return Result.Ok();
    }
}