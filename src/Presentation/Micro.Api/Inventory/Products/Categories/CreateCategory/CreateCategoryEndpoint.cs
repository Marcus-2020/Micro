using Micro.Api.Common.Endpoints;
using Micro.Inventory.Contracts.Products.Categories.CreateCategory;
using Micro.Inventory.Products.Categories.CreateCategory;

namespace Micro.Api.Inventory.Products.Categories.CreateCategoryEndpoint;

public class CreateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("", HandleAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        CreateCategoryRequest request, 
        ICreateCategoryHandler handler)
    {
        var response = await handler.HandleAsync(request);
        if (response.StatusCode == StatusCodes.Status200OK)
        {
            return Results.Ok(response.Data);
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