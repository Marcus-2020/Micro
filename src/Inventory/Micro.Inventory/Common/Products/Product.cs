using Micro.Core.Common;
using Micro.Core.Common.Entities;

namespace Micro.Inventory.Common.Products;

internal class Product : BaseEntity
{
    public Product(string sku, string name, string description, ProductTypeEnum productType, 
        ProductCategory category, ProductUnit unit, ProductPriceInfo priceInfo, bool active)
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue)
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
    
    public Product(Guid id, string sku, string name, string description, ProductTypeEnum productType, 
        ProductCategory category, ProductUnit unit, ProductPriceInfo priceInfo, bool active,
        DateTime createdAt, DateTime updatedAt) : base(id, createdAt, updatedAt)
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