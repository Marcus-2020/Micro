using Micro.Inventory.Products.Common.DTOs;
using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.Common.ValueObjects;

namespace Micro.Inventory.Products.Common.Extensions;

internal static class ProductExtensions
{
    public static Product ToProduct(this ProductDto productDto)
    {
        return new Product(
            productDto.Id,
            productDto.Sku,
            productDto.Name,
            productDto.Description,
            productDto.ProductType,
            new ProductCategory(productDto.CategoryId, productDto.CategoryName),
            new ProductUnit(productDto.UnitId, productDto.UnitName),
            new ProductPriceInfo(productDto.CostPrice, productDto.ProfitMargin, productDto.SellingPrice),
            productDto.IsActive, productDto.IsDeleted, productDto.CreatedAt, productDto.UpdatedAt, productDto.DeletedAt);
    }
    
    public static ProductDto ToProductDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.ProductType,
            product.Category.Id,
            product.Category.Name,
            product.Unit.Id,
            product.Unit.Name,
            product.PriceInfo.CostPrice,
            product.PriceInfo.ProfitMargin,
            product.PriceInfo.SellingPrice,
            product.CreatedAt,
            product.UpdatedAt,
            product.DeletedAt,
            product.IsActive);
    }
}