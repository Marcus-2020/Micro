using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Products.Categories.Common.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    bool IsActive)
{
    public bool IsDeleted => DeletedAt is not null;
}