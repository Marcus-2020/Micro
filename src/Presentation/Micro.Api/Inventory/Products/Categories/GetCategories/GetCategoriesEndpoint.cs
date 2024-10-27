using Micro.Api.Common.Endpoints;
using Micro.Inventory.Contracts.Products.Categories.GetCategories;
using Micro.Inventory.Products.Categories.GetCategories;

namespace Micro.Api.Inventory.Products.Categories.GetCategories;

public class GetCategoriesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("", HandleAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        int skip, int take, 
        IGetCategoriesHandler handler)
    {
        GetCategoriesRequest request = new GetCategoriesRequest(skip, take);
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