namespace Micro.Inventory.Contracts.Products.Categories.CreateCategory;

public record CreateCategoryRequest(
    string Name,
    string Description,
    bool IsActive);