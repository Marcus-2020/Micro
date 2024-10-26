namespace Micro.Inventory.Contracts.Categories.Common.Events;

public record CategoryCreatedEvent(
    string CategoryId,
    string Name,
    DateTime CreatedAt)
{
    public string Type => EventNames.CategoryCreated;
    public string Message => $"Category {CategoryId} created at {CreatedAt:O}";
    public string Origin => "Micro.Inventory";
    public string Version => "1.0.0";
}