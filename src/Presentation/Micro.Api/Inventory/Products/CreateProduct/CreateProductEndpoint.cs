using Micro.Api.Common.Endpoints;
using Micro.Inventory.Contracts.Products.CreateProduct;
using Micro.Inventory.Products.CreateProduct;

namespace Micro.Api.Inventory.Products.CreateProduct;

public class CreateProductEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("", HandleAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(CreateProductRequest request, ICreateProductHandler handler)
    {
        var response = await handler.HandleAsync(request);
        if (response.StatusCode == StatusCodes.Status201Created)
        {
            return Results.Created("", response.Data);
        }

        return response switch
        {
            { StatusCode: StatusCodes.Status422UnprocessableEntity } 
                => Results.UnprocessableEntity(response.Data),
            { StatusCode: StatusCodes.Status400BadRequest } 
                => Results.BadRequest(response.Data),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}