using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.Common.Enums;
using Micro.Inventory.Products.Common.ValueObjects;

namespace Micro.Inventory.Products.Common.DTOs;

public record ProductDto(
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
    public Guid Id
    {
        get => Id;
        set
        {
            if (Id != Guid.Empty) throw new ArgumentException("Id is already set");
            Id = value;
        } 
    }
    
    public bool IsDeleted => DeletedAt is not null;
}