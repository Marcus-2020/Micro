using Micro.Api.Common.Endpoints;
using Micro.Inventory.Products.CreateProduct;
using Micro.Inventory.Products.CreateProduct.Requests;

namespace Micro.Api.Inventory.Products.CreateProduct;

public class CreateProductEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/product", HandleAsync);
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