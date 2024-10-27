using Micro.Core.Common.Responses;
using Micro.Inventory.Contracts.Products.Categories.GetCategories;

namespace Micro.Inventory.Products.Categories.GetCategories;

public interface IGetCategoriesHandler
{
    Task<Response<GetCategoriesResponse>> HandleAsync(GetCategoriesRequest request);
}