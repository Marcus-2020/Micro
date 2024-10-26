namespace Micro.Inventory.Contracts.Categories.GetCategory;

public record GetCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    bool IsActive);