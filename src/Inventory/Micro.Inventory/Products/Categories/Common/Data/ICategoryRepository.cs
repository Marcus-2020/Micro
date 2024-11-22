using FluentResults;
using Micro.Core.Common.Data;
using Micro.Inventory.Products.Categories.Common.DTOs;
using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Products.Categories.Common.Data;

public interface ICategoryRepository
{
    Task<Result<CategoryDto>> GetByIdAsync(IDataContext dataContext, Guid categoryId);
    Task<Result<List<CategoryDto>>> GetAllAsync(IDataContext dataContext, int skip, int take);
    Task<Result<int>> CountAsync(IDataContext dataContext);
    Task<Result<(Guid Id, DateTime CreatedAt)>> AddAsync(IDataContext dataContext, CategoryDto category);
    Task<Result> UpdateAsync(IDataContext dataContext, CategoryDto category);
    Task<Result> DeleteAsync(IDataContext dataContext, Guid categoryId);
    Task<Result<List<CategoryDto>>> GetByNameAsync(IDataContext dataContext, string name);
}