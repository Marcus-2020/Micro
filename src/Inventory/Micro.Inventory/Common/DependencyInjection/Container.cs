using FluentValidation;
using Micro.Inventory.Contracts.Products.Categories.CreateCategory;
using Micro.Inventory.Contracts.Products.Categories.GetCategories;
using Micro.Inventory.Contracts.Products.CreateProduct;
using Micro.Inventory.Products.Categories.Common.Data;
using Micro.Inventory.Products.Categories.Common.Messaging;
using Micro.Inventory.Products.Categories.CreateCategory;
using Micro.Inventory.Products.Categories.GetCategories;
using Micro.Inventory.Products.Common.Data;
using Micro.Inventory.Products.Common.Messaging;
using Micro.Inventory.Products.CreateProduct;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Inventory.Common.DependencyInjection;

public static class Container
{
    public static void AddInventoryServices(this IServiceCollection sc)
    {
        sc.AddProductServices();
        sc.AddCategoryServices();
    }

    private static void AddProductServices(this IServiceCollection sc)
    {
        sc.AddSingleton<IProductMessageProducer, ProductMessageProducer>();
        
        sc.AddScoped<IProductRepository, ProductRepository>();
        
        sc.AddScoped<IValidator<CreateProductRequest>, CreateProductValidator>();
        sc.AddScoped<ICreateProductHandler, CreateProductHandler>();
    }
    
    private static void AddCategoryServices(this IServiceCollection sc)
    {
        sc.AddSingleton<ICategoryMessageProducer, CategoryMessageProducer>();
        
        sc.AddScoped<ICategoryRepository, CategoryRepository>();
        
        sc.AddScoped<IValidator<CreateCategoryRequest>, CreateCategoryValidator>();
        sc.AddScoped<ICreateCategoryHandler, CreateCategoryHandler>();
        
        sc.AddScoped<IValidator<GetCategoriesRequest>, GetCategoriesValidator>();
        sc.AddScoped<IGetCategoriesHandler, GetCategoriesHandler>();
    }
}