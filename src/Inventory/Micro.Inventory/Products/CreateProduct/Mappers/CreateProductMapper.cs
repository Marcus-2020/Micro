using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.Common.Enums;
using Micro.Inventory.Products.Common.ValueObjects;
using Micro.Inventory.Products.CreateProduct.Requests;
using Micro.Inventory.Products.CreateProduct.ValueObjects;

namespace Micro.Inventory.Products.CreateProduct.Mappers;

internal static class CreateProductMapper
{
    internal static Product ToProduct(this CreateProductRequest request)
    {
        return new Product(
            Guid.NewGuid(),
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