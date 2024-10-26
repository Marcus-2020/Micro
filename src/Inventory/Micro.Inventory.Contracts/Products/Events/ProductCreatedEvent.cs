using Micro.Core.Common.Infra.Messaging;
using Micro.Inventory.Contracts.Products.Events;

namespace Micro.Inventory.Products.Contracts.Events;

public record ProductCreatedEvent(
    string ProductId,
    string Sku,
    string ProductName,
    string ProductDescription,
    int ProductType,
    string CategoryId,
    string UnitId,
    decimal CostPrice,
    decimal ProfitMargin,
    decimal SellingPrice,
    DateTime CreatedAt) : IMessage
{
    public string Type => EventNames.ProductCreated;
    public string Message => $"Product {ProductId} created at {CreatedAt:O}";
    public string Origin => "Micro.Inventory";
    public string Version => "1.0.0";
}