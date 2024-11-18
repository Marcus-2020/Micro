using Micro.Inventory.Contracts.Products.CreateProduct;
using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.Common.Enums;
using Micro.Inventory.Products.Common.ValueObjects;

namespace Micro.Inventory.Products.CreateProduct;

internal static class CreateProductMapper
{
    internal static Product ToProduct(this CreateProductRequest request)
    {
        return new Product(
            Guid.Empty,
            request.Sku,
            request.Name,
            request.Description,
            (ProductTypeEnum)request.Type,
            new ProductCategory(Guid.Parse(request.CategoryId)),
            new ProductUnit(Guid.Parse(request.UnitId)),
            request.PriceInfo.ToPriceInfo(), true);
    }

    internal static ProductPriceInfo ToPriceInfo(this CreateProductPriceInfo priceInfo)
    {
        return new ProductPriceInfo(priceInfo.CostPrice, priceInfo.ProfitMargin, priceInfo.SellingPrice);
    }
}