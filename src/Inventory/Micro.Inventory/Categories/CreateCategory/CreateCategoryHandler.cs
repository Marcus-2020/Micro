using FluentValidation;
using Micro.Core.Common.Data;
using Micro.Core.Common.Responses;
using Micro.Inventory.Categories.Common.Data;
using Micro.Inventory.Categories.Common.Messaging;
using Micro.Inventory.Contracts.Categories.CreateCategory;

namespace Micro.Inventory.Categories.CreateCategory;

internal class CreateCategoryHandler : ICreateCategoryHandler
{
    private readonly IValidator<CreateCategoryRequest> _validator;
    private readonly IDataContextFactory _dataContextFactory;
    private readonly ICategoryRepository _productRepository;
    private readonly ICategoryMessageProducer _messagingProducer;
    
    public async Task<Response<CreateCategoryResponse>> HandleAsync(CreateCategoryRequest request)
    {
        throw new NotImplementedException();
    }
}