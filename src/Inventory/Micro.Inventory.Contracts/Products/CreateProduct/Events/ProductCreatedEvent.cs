using Micro.Core.Common.Infra.Messaging;

namespace Micro.Inventory.Contracts.Products.CreateProduct.Events;

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