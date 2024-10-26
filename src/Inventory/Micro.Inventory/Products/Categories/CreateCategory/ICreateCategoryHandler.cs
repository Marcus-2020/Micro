using Micro.Core.Common.Handlers;
using Micro.Core.Common.Responses;
using Micro.Inventory.Contracts.Products.Categories.CreateCategory;

namespace Micro.Inventory.Products.Categories.CreateCategory;

public interface ICreateCategoryHandler : IHandler<CreateCategoryRequest, Response<CreateCategoryResponse>> {}