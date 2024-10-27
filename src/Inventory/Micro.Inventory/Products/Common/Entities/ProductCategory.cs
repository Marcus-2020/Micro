using Micro.Core.Common.Entities;

namespace Micro.Inventory.Products.Common.Entities;

internal class ProductCategory : SoftDeletableEntity
{
    public ProductCategory()
        : base(Guid.Empty, DateTime.MinValue, null)
    {
        _name = "";
        _description = "";
    }
    
    public ProductCategory(Guid id, string name = "", string description = "")
        : base(id, DateTime.MinValue, null)
    {
        _name = name;
        _description = description;
    }
    
    public ProductCategory(Guid id, string name, string description, bool isActive)
        : base(id, DateTime.MinValue, null)
    {
        _name = name;
        _description = description;
        IsActive = isActive;
    }
    
    public ProductCategory(Guid id, string name, string description, bool isActive, bool isDeleted, 
        DateTime createdAt, DateTime? updatedAt = null, DateTime? deletedAt = null) 
        : base(id, createdAt, updatedAt, deletedAt)
    {
        _name = name;
        _description = description;
        IsActive = isActive;
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
    public bool IsActive { get; private set; }
}