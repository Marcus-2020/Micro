using FluentResults;
using Micro.Core.Common.Data;
using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Products.Categories.Common.Data;

internal interface ICategoryRepository
{
    Task<Result<ProductCategory>> GetByIdAsync(IDataContext dataContext, Guid categoryId);
    Task<Result<IEnumerable<ProductCategory>>> GetAllAsync(IDataContext dataContext, int skip, int take);
    Task<Result<int>> CountAsync(IDataContext dataContext);
    Task<Result<(Guid Id, DateTime CreatedAt)>> AddAsync(IDataContext dataContext, ProductCategory category);
    Task<Result> UpdateAsync(IDataContext dataContext, ProductCategory category);
    Task<Result> DeleteAsync(IDataContext dataContext, Guid categoryId);
    Task<Result<IEnumerable<ProductCategory>>> GetByNameAsync(IDataContext dataContext, string name);
}