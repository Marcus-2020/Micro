using Micro.Core.Common.Entities;

namespace Micro.Inventory.Common.Storages;

internal class Storage : SoftDeletableEntity
{
    public Storage(string name, StorageAddress? address, bool active)
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue)
    {
        _name = name;
        Address = address;
        Active = active;
    }

    public Storage(Guid id, string name, StorageAddress? address, bool active, 
        DateTime createdAt, DateTime updatedAt, bool isDeleted)
        : base(id, createdAt, updatedAt)
    {
        _name = name;
        Address = address;
        Active = active;
    }
    
    private string _name;
    public string Name
    {
        get => _name ??= "";
        private set => _name = value ?? "";
    }
    
    public StorageAddress? Address { get; private set; }

    public bool Active { get; private set; }
}