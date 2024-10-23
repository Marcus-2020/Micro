using Micro.Core.Common.Entities;

namespace Micro.Inventory.Products.Common.Entities;

internal class ProductCategory : SoftDeletableEntity
{
    public ProductCategory()
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue, false)
    {
        _name = "";
    }
    
    public ProductCategory(Guid id)
        : base(id, DateTime.MinValue, DateTime.MinValue, false)
    {
        _name = "";
    }
    
    public ProductCategory(Guid id, string name)
        : base(id, DateTime.MinValue, DateTime.MinValue, false)
    {
        _name = name;
    }
    
    public ProductCategory(string name, bool active, bool isDeleted)
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue, isDeleted)
    {
        _name = name;
        Active = active;
    }
    
    public ProductCategory(Guid id, string name, bool active, bool isDeleted, 
        DateTime createdAt, DateTime updatedAt) : base(id, createdAt, updatedAt, isDeleted)
    {
        _name = name;
        Active = active;
    }
    
    private string _name;
    public string Name
    {
        get => _name ??= "";
        private set => _name = value ?? "";
    }

    public bool Active { get; private set; }
}