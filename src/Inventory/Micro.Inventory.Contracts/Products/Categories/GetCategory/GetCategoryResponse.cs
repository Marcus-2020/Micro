namespace Micro.Inventory.Contracts.Products.Categories.GetCategory;

public record GetCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    bool IsActive);