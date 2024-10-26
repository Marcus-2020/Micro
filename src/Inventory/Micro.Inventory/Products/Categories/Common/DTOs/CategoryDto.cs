using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Products.Categories.Common.DTOs;

internal record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    bool IsActive)
{
    public bool IsDeleted => DeletedAt is not null;

    public ProductCategory ToProductCategory()
    {
        return new ProductCategory(
            Id, 
            Name, 
            Description,
            IsActive,
            IsDeleted,
            CreatedAt,
            UpdatedAt,
            DeletedAt);
    }
}