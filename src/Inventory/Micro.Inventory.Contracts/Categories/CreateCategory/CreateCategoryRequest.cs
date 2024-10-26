namespace Micro.Inventory.Contracts.Categories.CreateCategory;

public record CreateCategoryRequest(
    string Name,
    string Description,
    bool IsActive);