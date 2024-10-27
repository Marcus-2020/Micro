using System.Security.Claims;
using Micro.Api.Inventory.Products.Categories.GetCategories;
using Micro.Api.Inventory.Products.CreateProduct;
using Micro.Core.Telemetry;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using Serilog;

namespace Micro.Api.Common.Endpoints;

public static class Endpoint
{
    public static void UseAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
    
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("");

        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", ([FromServices] Tracer tracer) =>
            {
                using var span = tracer.StartActive("health-check");
                var logger = Log.Logger;
                logger.Information("Health check: OK");
                return new { message = "OK" };
            });
        
        endpoints.MapGroup("/private")
            .WithTags("Private")
            .MapGet("/", ([FromServices] Tracer tracer, HttpContext context) =>
            {
                return new {message = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value};
            }).RequireAuthorization();

        MapInventoryModuleEndpoints(endpoints);
            
        endpoints.MapGroup("v1/finances")
            .WithTags("Finances");
        
        endpoints.MapGroup("v1/sales")
            .WithTags("Sales");
        
        endpoints.MapGroup("v1/purchases")
            .WithTags("Purchases");
    }

    private static void MapInventoryModuleEndpoints(RouteGroupBuilder endpoints)
    {
        var inventoryGroup = endpoints.MapGroup("v1/inventory")
            .WithTags("Inventory");

        var invProdGroup = inventoryGroup.MapGroup("products")
            .WithTags("Products");
        
        invProdGroup.MapEndpoint<CreateProductEndpoint>();
        
        invProdGroup.MapGroup("categories")
            .WithTags("Product Categories")
            .MapEndpoint<GetCategoriesEndpoint>();
    }

    public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) 
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}