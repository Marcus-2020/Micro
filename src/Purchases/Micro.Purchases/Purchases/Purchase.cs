using Micro.Core.Common.Entities;
using Micro.Core.Common.ValueObjects;

namespace Micro.Purchases.Purchases;

internal class Purchase : SoftDeletableEntity
{
    public Purchase(int saleNumber, DateTime saleDate, PurchaseStatusEnum status, PriceInfo priceInfo, 
        List<PurchaseItem>? items = null) : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue, false)
    {
        PurchaseNumber = saleNumber;
        PurchaseDate = saleDate;
        Status = status;
        PriceInfo = priceInfo;
        _items = items ?? new();
    }
    
    public Purchase(Guid id, int saleNumber, DateTime saleDate, PurchaseStatusEnum status, PriceInfo priceInfo, 
        List<PurchaseItem> items, DateTime createdAt, DateTime updatedAt, bool isDeleted) : base(id, createdAt, updatedAt, isDeleted)
    {
        PurchaseNumber = saleNumber;
        PurchaseDate = saleDate;
        Status = status;
        PriceInfo = priceInfo;
        _items = items;
    }
    
    public int PurchaseNumber { get; set; }
    public DateTime PurchaseDate { get; set; }
    public PurchaseStatusEnum Status { get; set; }
    
    public PriceInfo PriceInfo { get; private set; }

    private List<PurchaseItem> _items;
    public IReadOnlyList<PurchaseItem> Items => _items;
}