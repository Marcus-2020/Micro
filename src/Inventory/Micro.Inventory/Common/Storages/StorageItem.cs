using Micro.Core.Common.Entities;

namespace Micro.Inventory.Common.Storages;

internal class StorageItem : BaseEntity
{
    public StorageItem(Guid productId, Guid storageId, decimal quantityStored) 
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue)
    {
        ProductId = productId;
        StorageId = storageId;
        QuantityStored = quantityStored;
    }
    
    public StorageItem(Guid id, Guid productId, Guid storageId, decimal quantityStored, 
        DateTime createdAt, DateTime updatedAt) 
        : base(id, createdAt, updatedAt)
    {
        ProductId = productId;
        StorageId = storageId;
        QuantityStored = quantityStored;
    }

    public Guid ProductId { get; private set; }
    
    public Guid StorageId { get; private set; }

    public decimal QuantityStored { get; set; }
}