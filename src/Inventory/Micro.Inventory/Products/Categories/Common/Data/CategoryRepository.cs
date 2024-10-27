using Dapper;
using FluentResults;
using Micro.Core.Common.Data;
using Micro.Inventory.Products.Categories.Common.Data.Errors;
using Micro.Inventory.Products.Categories.Common.DTOs;
using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Products.Categories.Common.Data;

internal class CategoryRepository : ICategoryRepository
{
    public async Task<Result<ProductCategory>> GetByIdAsync(IDataContext dataContext, Guid categoryId)
    {
        string sql = 
            """
            SELECT c.id AS Id, c.name AS Name, c.description AS Description,
                   c.created_at AS CreatedAt, c.updated_at AS UpdatedAt, c.deleted_at AS DeletedAt, 
                   c.is_active as IsActive
            FROM inventory.categories c
            WHERE c.id = @id
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }

        try
        {
            var categoryDto = await dataContext.Connection
                .QueryFirstAsync<CategoryDto>(sql, 
                    new {id = categoryId},
                    dataContext.Transaction);
            
            if (categoryDto.Id.ToString() is null or "") 
                return Result
                    .Fail(new Error("Category not found")
                        .WithMetadata("CATEGORY_NOT_FOUND", string.Empty));
            
            return Result.Ok(categoryDto.ToProductCategory());
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetCategoryByIdError(ex, categoryId.ToString()));
        }
    }

    public async Task<Result<IEnumerable<ProductCategory>>> GetAllAsync(IDataContext dataContext)
    {
        string sql = 
            """
            SELECT c.id AS Id, c.name AS Name, c.description AS Description,
                   c.created_at AS CreatedAt, c.updated_at AS UpdatedAt, c.deleted_at AS DeletedAt, 
                   c.is_active as IsActive
            FROM inventory.categories c
            WHERE c.deleted_at IS NULL
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }

        try
        {
            var categoriesDto = (await dataContext.Connection
                .QueryAsync<CategoryDto>(sql, dataContext.Transaction)).ToList();
            
            return Result.Ok(categoriesDto.Select(x => x.ToProductCategory()));
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetAllCategoriesError(ex));
        }
    }

    public async Task<Result<(Guid Id, DateTime CreatedAt)>> AddAsync(IDataContext dataContext, ProductCategory category)
    {
        var sql = 
            """
            INSERT INTO inventory.categories (id, name, description, created_at, is_active)
            VALUES (@id, @name, @description, @createdAt, @isActive);
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
                    id = category.Id,
                    name = category.Name,
                    description = category.Description,
                    createdAt,
                    isActive = true,
                },
                dataContext.Transaction);

            return (category.Id, createdAt);
        }
        catch (Exception ex)
        {
            return Result.Fail(new AddCategoryError(ex));
        }
    }

    public async Task<Result> UpdateAsync(IDataContext dataContext, ProductCategory category)
    {
        var sql = 
            """
            UPDATE inventory.categories 
            SET 
                name = @name, 
                description = @description, 
                updated_at = @updatedAt
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
                    id = category.Id,
                    name = category.Name,
                    description = category.Description,
                    updatedAt = DateTime.UtcNow,
                },
                dataContext.Transaction);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new UpdateCategoryError(ex, category.Id.ToString()));
        }
    }

    public async Task<Result> DeleteAsync(IDataContext dataContext, Guid categoryId)
    {
        var sql = 
            """
            UPDATE inventory.categories 
            SET  
                deleted_at = @updatedAt
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
                    id = categoryId,
                    deletedAt = DateTime.UtcNow,
                },
                dataContext.Transaction);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new DeleteCategoryError(ex, categoryId.ToString()));
        }
    }

    public async Task<Result<IEnumerable<ProductCategory>>> GetByNameAsync(IDataContext dataContext, string name)
    {
        string sql = 
            """
            SELECT c.id AS Id, c.name AS Name, c.description AS Description,
                   c.created_at AS CreatedAt, c.updated_at AS UpdatedAt, c.deleted_at AS DeletedAt, 
                   c.is_active as IsActive
            FROM inventory.categories c
            WHERE c.name = @name AND c.deleted_at IS NULL
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }

        try
        {
            var categoriesDto = (await dataContext.Connection
                .QueryAsync<CategoryDto>(sql, new { name }, dataContext.Transaction)).ToList();
            
            return Result.Ok(categoriesDto.Select(x => x.ToProductCategory()));
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetAllCategoriesError(ex));
        }
    }
}