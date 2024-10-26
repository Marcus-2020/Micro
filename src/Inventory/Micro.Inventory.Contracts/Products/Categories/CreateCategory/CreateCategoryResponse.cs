namespace Micro.Inventory.Contracts.Products.Categories.CreateCategory;

public record CreateCategoryResponse(
    string Id,
    bool IsActive,
    DateTime CreatedAt);