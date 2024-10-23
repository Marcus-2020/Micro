using Micro.Inventory.Products.Common.Enums;

namespace Micro.Inventory.Products.Common.DTOs;

internal record ProductDto(
    string Id,
    string Sku,
    string Name,
    string Description,
    ProductTypeEnum ProductType,
    string CategoryId,
    string CategoryName,
    string UnitId,
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
}