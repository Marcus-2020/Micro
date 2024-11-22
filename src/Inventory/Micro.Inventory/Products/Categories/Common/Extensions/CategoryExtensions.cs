using Micro.Inventory.Products.Categories.Common.DTOs;
using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Products.Categories.Common.Extensions;

internal static class CategoryExtensions
{
    public static ProductCategory ToProductCategory(this CategoryDto dto)
    {
        return new ProductCategory(
            dto.Id, 
            dto.Name, 
            dto.Description,
            dto.IsActive,
            dto.IsDeleted,
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.DeletedAt);
    }
    
    public static CategoryDto ToCategoryDto(this ProductCategory productCategory)
    {
        return new CategoryDto(
            productCategory.Id,
            productCategory.Name,
            productCategory.Description,
            productCategory.CreatedAt,
            productCategory.UpdatedAt,
            productCategory.DeletedAt,
            productCategory.IsActive);
    }
}