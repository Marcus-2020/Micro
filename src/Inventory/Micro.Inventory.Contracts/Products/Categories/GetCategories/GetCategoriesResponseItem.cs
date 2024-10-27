namespace Micro.Inventory.Contracts.Products.Categories.GetCategories;

public record GetCategoriesResponseItem(
    Guid Id,
    string Name,
    string Description,
    bool IsActive);