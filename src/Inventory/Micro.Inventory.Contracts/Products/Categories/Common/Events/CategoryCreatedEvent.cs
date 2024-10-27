using Micro.Core.Common.Infra.Messaging;

namespace Micro.Inventory.Contracts.Products.Categories.Common.Events;

public record CategoryCreatedEvent(
    string CategoryId,
    string Name,
    DateTime CreatedAt) : IMessage
{
    public string Type => EventNames.CategoryCreated;
    public string Message => $"Category {CategoryId} created at {CreatedAt:O}";
    public string Origin => "Micro.Inventory";
    public string Version => "1.0.0";
}