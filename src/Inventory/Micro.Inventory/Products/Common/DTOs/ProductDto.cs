using Micro.Inventory.Products.Common.Enums;

namespace Micro.Inventory.Products.Common.DTOs;

internal class ProductDto(
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
    public Guid Id { get; init; } = Id;
    public string Sku { get; init; } = Sku;
    public string Name { get; init; } = Name;
    public string Description { get; init; } = Description;
    public ProductTypeEnum ProductType { get; init; } = ProductType;
    public Guid CategoryId { get; init; } = CategoryId;
    public string CategoryName { get; init; } = CategoryName;
    public Guid UnitId { get; init; } = UnitId;
    public string UnitName { get; init; } = UnitName;
    public decimal CostPrice { get; init; } = CostPrice;
    public decimal ProfitMargin { get; init; } = ProfitMargin;
    public decimal SellingPrice { get; init; } = SellingPrice;
    public DateTime CreatedAt { get; init; } = CreatedAt;
    public DateTime? UpdatedAt { get; init; } = UpdatedAt;
    public DateTime? DeletedAt { get; init; } = DeletedAt;
    public bool IsActive { get; init; } = IsActive;
    public bool IsDeleted => DeletedAt is not null;
}