using Micro.Core.Common.Entities;

namespace Micro.Sales.Sales;

public class Sale : BaseEntity
{
    public Sale(int saleNumber, DateTime saleDate, SaleStatusEnum status, PriceInfo priceInfo, List<SaleItem>? items = null)
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue)
    {
        SaleNumber = saleNumber;
        SaleDate = saleDate;
        Status = status;
        PriceInfo = priceInfo;
        _items = items ?? new();
    }
    
    public Sale(Guid id, int saleNumber, DateTime saleDate, SaleStatusEnum status, PriceInfo priceInfo, 
        List<SaleItem> items, DateTime createdAt, DateTime updatedAt) : base(id, createdAt, updatedAt)
    {
        SaleNumber = saleNumber;
        SaleDate = saleDate;
        Status = status;
        PriceInfo = priceInfo;
        _items = items;
    }

    public int SaleNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public SaleStatusEnum Status { get; set; }
    
    public PriceInfo PriceInfo { get; private set; }

    private List<SaleItem> _items;
    public IReadOnlyList<SaleItem> Items => _items;
}