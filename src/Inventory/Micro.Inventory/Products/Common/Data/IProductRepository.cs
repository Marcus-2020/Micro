using FluentResults;
using Micro.Core.Common.Data;
using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Products.Common.Data;

internal interface IProductRepository
{
    Task<Result<Product>> GetByIdAsync(IDataContext dataContext, string productId);
    Task<Result<IEnumerable<Product>>> GetAllAsync(IDataContext dataContext);
    Task<Result<(Guid Id, DateTime CreatedAt)>> AddAsync(IDataContext dataContext, Product product);
    Task<Result> UpdateAsync(IDataContext dataContext, Product product);
    Task<Result> DeleteAsync(IDataContext dataContext, string productId);
    Task<Result<IEnumerable<Product>>> GetByNameOrSku(IDataContext dataContext, string sku, string name);
}