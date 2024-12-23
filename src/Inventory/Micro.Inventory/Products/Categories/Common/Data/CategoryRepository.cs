using Dapper;
using FluentResults;
using Micro.Core.Common.Data;
using Micro.Inventory.Products.Categories.Common.Data.Errors;
using Micro.Inventory.Products.Categories.Common.DTOs;
using Micro.Inventory.Products.Common.Entities;

namespace Micro.Inventory.Products.Categories.Common.Data;

internal class CategoryRepository : ICategoryRepository
{
    public async Task<Result<CategoryDto>> GetByIdAsync(IDataContext dataContext, Guid categoryId)
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
            
            return Result.Ok(categoryDto);
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetCategoryByIdError(ex, categoryId.ToString()));
        }
    }

    public async Task<Result<List<CategoryDto>>> GetAllAsync(IDataContext dataContext, int skip, int take)
    {
        string sql = 
            """
            SELECT c.id AS Id, c.name AS Name, c.description AS Description,
                   c.created_at AS CreatedAt, c.updated_at AS UpdatedAt, c.deleted_at AS DeletedAt, 
                   c.is_active as IsActive
            FROM inventory.categories c
            WHERE c.is_active = @isActive AND c.deleted_at IS NULL
            LIMIT @take OFFSET @skip
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }

        try
        {
            var categoriesDto = (await dataContext.Connection
                .QueryAsync<CategoryDto>(sql, new {isActive = true, skip, take}, dataContext.Transaction)).ToList();
            
            return Result.Ok(categoriesDto);
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetAllCategoriesError(ex));
        }
    }

    public async Task<Result<int>> CountAsync(IDataContext dataContext)
    {
        string sql = 
            """
            SELECT COUNT(c.id)
            FROM inventory.categories c
            WHERE c.is_active = @isActive AND c.deleted_at IS NULL
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }

        try
        {
            var count = await dataContext.Connection
                .ExecuteScalarAsync<int>(sql, new {isActive = true}, dataContext.Transaction);
            return count;
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetAllCategoriesError(ex));
        }
    }

    public async Task<Result<(Guid Id, DateTime CreatedAt)>> AddAsync(IDataContext dataContext, CategoryDto category)
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
            var id = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;
            
            await dataContext.Connection.ExecuteAsync(
                sql,
                new
                {
                    id,
                    name = category.Name,
                    description = category.Description,
                    createdAt,
                    isActive = true,
                },
                dataContext.Transaction);

            return (id, createdAt);
        }
        catch (Exception ex)
        {
            return Result.Fail(new AddCategoryError(ex));
        }
    }

    public async Task<Result> UpdateAsync(IDataContext dataContext, CategoryDto category)
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

    public async Task<Result<List<CategoryDto>>> GetByNameAsync(IDataContext dataContext, string name)
    {
        string sql = 
            """
            SELECT c.id AS Id, c.name AS Name, c.description AS Description,
                   c.created_at AS CreatedAt, c.updated_at AS UpdatedAt, c.deleted_at AS DeletedAt, 
                   c.is_active as IsActive
            FROM inventory.categories c
            WHERE c.name = @name AND c.is_active = @isActive AND c.deleted_at IS NULL
            """;

        if (dataContext is { IsConnectionOpen: false } || dataContext.Connection is null)
        {
            return Result.Fail(DataContextErrors.ConnectionNotOpenOrNull);
        }

        try
        {
            var categoriesDto = (await dataContext.Connection
                .QueryAsync<CategoryDto>(sql, new { name, isActive = true }, dataContext.Transaction)).ToList();
            
            return Result.Ok(categoriesDto);
        }
        catch (Exception ex)
        {
            return Result.Fail(new GetAllCategoriesError(ex));
        }
    }
}