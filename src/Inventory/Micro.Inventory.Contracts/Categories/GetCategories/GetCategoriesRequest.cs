using Micro.Core.Common.Requests;

namespace Micro.Inventory.Contracts.Categories.GetCategories;

public record GetCategoriesRequest(int Skip, int Take) : PagedRequest(Skip, Take);