using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.Common.Enums;
using Micro.Inventory.Products.Common.ValueObjects;

namespace Micro.Inventory.Products.Common.DTOs;

internal record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string Description,
    ProductTypeEnum ProductType,
    Guid CategoryId,
    string CategoryName,
    Guid UnitId,
    string UnitName,
    decimal CostPrice,
    decimal ProfitMargin,
    decimal SellingPrice,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    bool IsActive)
{
    public bool IsDeleted => DeletedAt is not null;

    public Product ToProduct()
    {
        return new Product(
            Id,
            Sku,
            Name,
            Description,
            ProductType,
            new ProductCategory(CategoryId, CategoryName),
            new ProductUnit(UnitId, UnitName),
            new ProductPriceInfo(CostPrice, ProfitMargin, SellingPrice),
            IsActive, IsDeleted, CreatedAt, UpdatedAt, DeletedAt);
    }
}