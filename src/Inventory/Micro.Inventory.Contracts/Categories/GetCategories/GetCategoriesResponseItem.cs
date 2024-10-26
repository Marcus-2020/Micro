namespace Micro.Inventory.Contracts.Categories.GetCategories;

public record GetCategoriesResponseItem(
    Guid Id,
    string Name,
    string Descriptions,
    bool IsActive);