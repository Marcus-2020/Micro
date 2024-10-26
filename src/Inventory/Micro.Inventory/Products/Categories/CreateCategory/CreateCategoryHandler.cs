using FluentValidation;
using Micro.Core.Common.Data;
using Micro.Core.Common.Responses;
using Micro.Inventory.Contracts.Products.Categories.CreateCategory;
using Micro.Inventory.Products.Categories.Common.Data;
using Micro.Inventory.Products.Categories.Common.Messaging;

namespace Micro.Inventory.Products.Categories.CreateCategory;

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