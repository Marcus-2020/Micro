using Dapper;
using FluentResults;
using Micro.Core.Common.Data;
using Micro.Inventory.Products.Common.Data.Errors;
using Micro.Inventory.Products.Common.DTOs;
using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.Common.ValueObjects;

namespace Micro.Inventory.Products.Common.Data;

internal class ProductRepository : IProductRepository
{
    public async Task<Result<Product>> GetByIdAsync(IDataContext dataContext, string productId)
    {
        var sql = 
            """
            SELECT p.id AS Id, p.sku AS Sku, p.name AS Name, p.description AS Description, p.product_type AS ProductType,
                   c.id AS CategoryId, c.name AS CategoryName, u.id AS UnitId, u.name AS UnitName,
                   p.cost_price AS CostPrice, p.profit_margin AS ProfitMargin, p.selling_price AS SellingPrice,
                   p.created_at AS CreatedAt, p.updated_at AS UpdatedAt, p.deleted_at AS DeletedAt, 
                   p.is_active as IsActive
            FROM inventory.products p
                LEFT JOIN inventory.categories c ON p.category_id = c.id
                LEFT JOIN inventory.units u ON p.unit_id = u.id
            WHERE Id = @id
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }
        
        try
        {
            var productDto = await dataContext.Connection
                .QueryFirstAsync<ProductDto>(
                    sql,
                    new {id = productId},
                    transaction: dataContext.Transaction);
            
            if (productDto.Id.ToString() is null or "") 
                return Result
                    .Fail(new Error("Product not found")
                    .WithMetadata("PRODUCT_NOT_FOUND", string.Empty));
            
            return Result.Ok(new Product(productDto));
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetProductByIdError(ex, productId));
        }
    }

    public async Task<Result<IEnumerable<Product>>> GetAllAsync(IDataContext dataContext)
    {
        var sql = 
            """
            SELECT p.id AS Id, p.sku AS Sku, p.name AS Name, p.description AS Description, p.product_type AS ProductType,
                   c.id AS CategoryId, c.name AS CategoryName, u.id AS UnitId, u.name AS UnitName,
                   p.cost_price AS CostPrice, p.profit_margin AS ProfitMargin, p.selling_price AS SellingPrice,
                   p.created_at AS CreatedAt, p.updated_at AS UpdatedAt, p.deleted_at AS DeletedAt, 
                   p.is_active as IsActive
            FROM inventory.products p
                LEFT JOIN inventory.categories c ON p.category_id = c.id
                LEFT JOIN inventory.units u ON p.unit_id = u.id
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }
        
        try
        {
            var productDtos = (await dataContext.Connection
                .QueryAsync<ProductDto>(
                    sql,
                    transaction: dataContext.Transaction)).ToList();
            
            return Result.Ok(productDtos.Select(x => new Product(x)));
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetAllProductsError(ex));
        }
    }

    public async Task<Result<(Guid Id, DateTime CreatedAt)>> AddAsync(IDataContext dataContext, Product product)
    {
        var sql = 
            """
            INSERT INTO inventory.products (id, sku, name, description, product_type, category_id, unit_id, cost_price, profit_margin, selling_price, created_at, is_active)
            VALUES (@id, @sku, @name, @description, @productType, @categoryId, @unitId, @costPrice, @profitMargin, @sellingPrice, @createdAt, @isActive);
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }
        
        try
        {
            var createdAt = DateTime.UtcNow;
            
            await dataContext.Connection.ExecuteAsync(
                sql,
                new
                {
                    id = product.Id,
                    sku = product.Sku,
                    name = product.Name,
                    description = product.Description,
                    productType = (int)product.ProductType,
                    categoryId = product.Category.Id,
                    unitId = product.Unit.Id,
                    costPrice = product.PriceInfo.CostPrice,
                    profitMargin = product.PriceInfo.ProfitMargin,
                    sellingPrice = product.PriceInfo.SellingPrice,
                    createdAt,
                    isActive = true,
                },
                dataContext.Transaction);

            return (product.Id, createdAt);
        }
        catch (Exception ex)
        {
            return Result.Fail(new AddProductError(ex));
        }
    }

    public async Task<Result> UpdateAsync(IDataContext dataContext, Product product)
    {
        var sql = 
            """
            UPDATE inventory.products 
            SET 
                sku = @sku, 
                name = @name, 
                description = @description, 
                product_type = @productType, 
                category_id = @categoryId, 
                unit_id = @unitId, 
                cost_price = @costPrice, 
                profit_margin = @profitMargin, 
                selling_price = @sellingPrice
            WHERE id = @id
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }
        
        try
        {
            await dataContext.Connection.ExecuteAsync(
                sql,
                new
                {
                    id = product.Id.ToString(),
                    sku = product.Sku,
                    name = product.Name,
                    description = product.Description,
                    productType = (int)product.ProductType,
                    categoryId = product.Category.Id.ToString(),
                    unitId = product.Unit.Id.ToString(),
                    costPrice = product.PriceInfo.CostPrice,
                    profitMargin = product.PriceInfo.ProfitMargin,
                    sellingPrice = product.PriceInfo.SellingPrice,
                },
                dataContext.Transaction);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new AddProductError(ex));
        }
    }

    public async Task<Result> DeleteAsync(IDataContext dataContext, string productId)
    {
        var sql = 
            """
            UPDATE inventory.products 
            SET 
                deleted_at = @deletedAt
            WHERE id = @id
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }
        
        try
        {
            await dataContext.Connection.ExecuteAsync(
                sql,
                new
                {
                    id = productId,
                    deletedAt = DateTime.UtcNow,
                },
                dataContext.Transaction);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new AddProductError(ex));
        }
    }

    public async Task<Result<IEnumerable<Product>>> GetByNameOrSku(IDataContext dataContext, string sku, string name)
    {
        var sql = 
            """
            SELECT p.id AS Id, p.sku AS Sku, p.name AS Name, p.description AS Description, p.product_type AS ProductType,
                   c.id AS CategoryId, c.name AS CategoryName, u.id AS UnitId, u.name AS UnitName,
                   p.cost_price AS CostPrice, p.profit_margin AS ProfitMargin, p.selling_price AS SellingPrice,
                   p.created_at AS CreatedAt, p.updated_at AS UpdatedAt, p.deleted_at AS DeletedAt, 
                   p.is_active as IsActive
            FROM inventory.products p
                LEFT JOIN inventory.categories c ON p.category_id = c.id
                LEFT JOIN inventory.units u ON p.unit_id = u.id
            WHERE p.Sku = @sku OR p.Name = @name
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }
        
        try
        {
            var productDtos = (await dataContext.Connection
                .QueryAsync<ProductDto>(
                    sql,
                    new {sku, name},
                    transaction: dataContext.Transaction)).ToList();
            
            return Result.Ok(productDtos.Select(x => new Product(x)));
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetAllProductsError(ex));
        }
    }
}