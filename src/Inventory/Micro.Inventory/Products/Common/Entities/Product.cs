using FluentResults;
using Micro.Core.Common.Entities;
using Micro.Inventory.Products.Common.Enums;
using Micro.Inventory.Products.Common.ValueObjects;

namespace Micro.Inventory.Products.Common.Entities;

internal class Product : SoftDeletableEntity
{
    public Product(Guid id, string sku, string name, string description, ProductTypeEnum productType, 
        ProductCategory category, ProductUnit unit, ProductPriceInfo priceInfo, bool isActive)
        : base(id, DateTime.MinValue, DateTime.MinValue)
    {
        Sku = sku;
        Name = name;
        Description = description;
        ProductType = productType;
        Category = category;
        Unit = unit;
        PriceInfo = priceInfo;
        IsActive = isActive;
    }
    
    public Product(Guid id, string sku, string name, string description, ProductTypeEnum productType, 
        ProductCategory category, ProductUnit unit, ProductPriceInfo priceInfo, bool isActive, bool isDeleted,
        DateTime createdAt, DateTime? updatedAt = null, DateTime? deletedAt = null) 
        : base(id, createdAt, updatedAt, deletedAt)
    {
        Sku = sku;
        Name = name;
        Description = description;
        ProductType = productType;
        Category = category;
        Unit = unit;
        PriceInfo = priceInfo;
        IsActive = isActive;
    }

    private string _sku;
    public string Sku
    {
        get => _sku ??= "";
        private set => _sku = value ?? "";
    }
    
    private string _name;
    public string Name
    {
        get => _name ??= "";
        private set => _name = value ?? "";
    }
    
    private string _description;
    public string Description
    {
        get => _description ??= "";
        private set => _description = value ?? "";
    }

    public ProductTypeEnum ProductType { get; private set; }
    
    private ProductCategory _category;
    public ProductCategory Category
    {
        get => _category ??= new();
        private set => _category = value ?? new();
    }
    
    private ProductUnit _unit;
    public ProductUnit Unit
    {
        get => _unit ??= new();
        private set => _unit = value ?? new();
    }

    public bool IsActive { get; set; }

    public ProductPriceInfo PriceInfo { get; private set; }
}