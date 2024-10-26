using Micro.Core.Common.Responses;

namespace Micro.Inventory.Contracts.Categories.GetCategories;

public record GetCategoriesResponse(
    IReadOnlyList<GetCategoriesResponseItem> Categories, int Count, int Skip, int Take) 
    : PagedResponse(Count, Skip, Take);