using Dapper;
using FluentResults;
using Micro.Core.Common.Data;
using Micro.Inventory.Products.Common.DTOs;
using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Categories.Common.Data;

internal class CategoryRepository : ICategoryRepository
{
    public async Task<Result<ProductCategory>> GetByIdAsync(IDataContext dataContext, string categoryId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<ProductCategory>>> GetAllAsync(IDataContext dataContext)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<(Guid Id, DateTime CreatedAt)>> AddAsync(IDataContext dataContext, ProductCategory category)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UpdateAsync(IDataContext dataContext, ProductCategory category)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteAsync(IDataContext dataContext, string categoryId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<ProductCategory>>> GetByName(IDataContext dataContext, string name)
    {
        throw new NotImplementedException();
    }
}