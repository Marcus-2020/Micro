using FluentValidation;
using Micro.Inventory.Products.CreateProduct;
using Micro.Inventory.Products.CreateProduct.Requests;
using Micro.Inventory.Products.CreateProduct.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Inventory.DependencyInjection;

public static class Container
{
    public static void AddInventoryServices(this IServiceCollection sc)
    {
        sc.AddScoped<IValidator<CreateProductRequest>, CreateProductValidator>();
        sc.AddScoped<ICreateProductHandler, CreateProductHandler>();
    }
}