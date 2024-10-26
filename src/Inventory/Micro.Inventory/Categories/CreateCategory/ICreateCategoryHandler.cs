using Micro.Core.Common.Handlers;
using Micro.Core.Common.Responses;
using Micro.Inventory.Contracts.Categories.CreateCategory;

namespace Micro.Inventory.Categories.CreateCategory;

public interface ICreateCategoryHandler : IHandler<CreateCategoryRequest, Response<CreateCategoryResponse>> {}