namespace Micro.Inventory.Contracts.Products.Categories.GetCategories;

public record GetCategoriesResponseItem(
    Guid Id,
    string Name,
    string Descriptions,
    bool IsActive);