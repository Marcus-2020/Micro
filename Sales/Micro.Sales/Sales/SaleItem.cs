using Micro.Core.Common.Entities;

namespace Micro.Sales.Sales;

public class SaleItem : BaseEntity
{
    public SaleItem(Guid productId, string description, PriceInfo priceInfo) 
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue)
    {
        ProductId = productId;
        Description = description;
        PriceInfo = priceInfo;
    }
    
    public SaleItem(Guid id, Guid productId, string description, PriceInfo priceInfo,
        DateTime createdAt, DateTime updatedAt) : base(id, createdAt, updatedAt)
    {
        ProductId = productId;
        Description = description;
        PriceInfo = priceInfo;
    }

    public Guid ProductId { get; set; }

    private string _description;
    public string Description
    {
        get => _description ??= "";
        private set => _description = value ?? "";
    }

    public PriceInfo PriceInfo { get; private set; }
}