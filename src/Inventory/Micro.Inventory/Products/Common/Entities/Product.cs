using Micro.Core.Common.Entities;
using Micro.Inventory.Products.Common.DTOs;
using Micro.Inventory.Products.Common.Enums;
using Micro.Inventory.Products.Common.ValueObjects;

namespace Micro.Inventory.Products.Common.Entities;

internal class Product : SoftDeletableEntity
{
    public Product(Guid id, string sku, string name, string description, ProductTypeEnum productType, 
        ProductCategory category, ProductUnit unit, ProductPriceInfo priceInfo, bool active)
        : base(id, DateTime.MinValue, DateTime.MinValue, false)
    {
        Sku = sku;
        Name = name;
        Description = description;
        ProductType = productType;
        Category = category;
        Unit = unit;
        PriceInfo = priceInfo;
        Active = active;
    }
    
    public Product(ProductDto product)
        : base(product.Id, product.CreatedAt, product.UpdatedAt, product.IsDeleted)
    {
        Sku = product.Sku;
        Name = product.Name;
        Description = product.Description;
        ProductType = product.ProductType;
        Active = product.IsActive;
        
        Category = new ProductCategory(product.CategoryId, product.CategoryName);
        Unit = new ProductUnit(product.UnitId, product.UnitName);
        PriceInfo = new ProductPriceInfo(product.CostPrice, product.ProfitMargin, product.SellingPrice);
    }
    
    public Product(Guid id, string sku, string name, string description, ProductTypeEnum productType, 
        ProductCategory category, ProductUnit unit, ProductPriceInfo priceInfo, bool active,
        DateTime createdAt, DateTime updatedAt, bool isDeleted) : base(id, createdAt, updatedAt, isDeleted)
    {
        Sku = sku;
        Name = name;
        Description = description;
        ProductType = productType;
        Category = category;
        Unit = unit;
        PriceInfo = priceInfo;
        Active = active;
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

    public bool Active { get; set; }

    public ProductPriceInfo PriceInfo { get; private set; }
}