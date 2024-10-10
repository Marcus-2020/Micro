using Micro.Api.Inventory.Products.CreateProduct;
using Micro.Core.Telemetry;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using Serilog;

namespace Micro.Api.Common.Endpoints;

public static class Endpoint
{
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
                return new {message = "OK"};
            });

        endpoints.MapGroup("v1/inventory")
            .WithTags("Inventory")
            .MapEndpoint<CreateProductEndpoint>();
            
        endpoints.MapGroup("v1/finances")
            .WithTags("Finances");
        
        endpoints.MapGroup("v1/sales")
            .WithTags("Sales");
        
        endpoints.MapGroup("v1/purchases")
            .WithTags("Purchases");
    }

    public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) 
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}