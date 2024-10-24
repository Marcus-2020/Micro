using Micro.Core.Common.Infra.Messaging;
using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.Common.Enums;

namespace Micro.Inventory.Products.CreateProduct.Events;

internal class ProductCreatedEvent : IMessage
{
    public string Type => "Inventory.ProductCreated";
    public string Message { get; init; }
    public string ProductId { get; init; }
    public string ProductName { get; init; }
    public string ProductDescription { get; init; }
    public ProductTypeEnum ProductType { get; init; }
    public string CategoryId { get; init; }
    public string UnitId { get; init; }
    public decimal CostPrice { get; init; }
    public decimal ProfitMargin { get; init; }
    public decimal SellingPrice { get; init; }
    public DateTime CreatedAt { get; init; }

    public ProductCreatedEvent(string message, Product product)
    {
        Message = message;
        
        ProductId = product.Id.ToString();
        ProductName = product.Name;
        ProductDescription = product.Description;
        ProductType = product.ProductType;
        CategoryId = product.Category.Id.ToString();
        UnitId = product.Unit.Id.ToString();
        CostPrice = product.PriceInfo.CostPrice;
        ProfitMargin = product.PriceInfo.ProfitMargin;
        SellingPrice = product.PriceInfo.SellingPrice;
        CreatedAt = product.CreatedAt;
    }
}