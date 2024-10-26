namespace Micro.Inventory.Contracts.Categories.CreateCategory;

public record CreateCategoryResponse(
    string Id,
    bool IsActive,
    DateTime CreatedAt);