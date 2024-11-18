using FluentResults;
using Micro.Core.Common.Data;
using Micro.Inventory.Products.Common.DTOs;
using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Products.Common.Data;

public interface IProductRepository
{
    Task<Result<ProductDto>> GetByIdAsync(IDataContext dataContext, Guid productId);
    Task<Result<IEnumerable<ProductDto>>> GetAllAsync(IDataContext dataContext);
    Task<Result<(Guid Id, DateTime CreatedAt)>> AddAsync(IDataContext dataContext, ProductDto product);
    Task<Result> UpdateAsync(IDataContext dataContext, ProductDto product);
    Task<Result> DeleteAsync(IDataContext dataContext, Guid productId);
    Task<Result<IEnumerable<ProductDto>>> GetByNameOrSku(IDataContext dataContext, string sku, string name);
}