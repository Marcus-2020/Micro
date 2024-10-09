using Micro.Core.Common;
using Micro.Core.Common.Entities;

namespace Micro.Inventory.Common.Products;

internal class ProductUnit : SoftDeletableEntity
{
    public ProductUnit()
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue, false)
    {
        _name = "";
        _symbol = "";
    }
    
    public ProductUnit(string name, string symbol, bool active)
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue, false)
    {
        _name = name;
        _symbol = symbol;
        Active = active;
    }
    
    public ProductUnit(Guid id, string name, string symbol, bool active, bool isDeleted, 
        DateTime createdAt, DateTime updatedAt) : base(id, createdAt, updatedAt, isDeleted)
    {
        _name = name;
        _symbol = symbol;
        Active = active;
    }

    private string _name;
    public string Name
    {
        get => _name ??= "";
        private set => _name = value ?? "";
    }
    
    private string _symbol;
    public string Symbol
    {
        get => _symbol ??= "";
        private set => _symbol = value ?? "";
    }
    
    public bool Active { get; private set; }
}