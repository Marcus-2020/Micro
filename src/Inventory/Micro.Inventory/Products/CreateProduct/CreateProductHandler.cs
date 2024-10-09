using FluentResults;
using FluentValidation;
using Micro.Core.Common.Handlers;
using Micro.Core.Common.Responses;

namespace Micro.Inventory.Products.CreateProduct;

public interface ICreateProductHandler : IHandler<CreateProductRequest, Response<CreateProductResponse>>
{
}

public class CreateProductHandler : ICreateProductHandler
{
    private readonly IValidator<CreateProductRequest> _validator;

    public CreateProductHandler(IValidator<CreateProductRequest> validator)
    {
        _validator = validator;
    }

    public async Task<Response<CreateProductResponse>> HandleAsync(CreateProductRequest request)
    {
        var results = _validator.Validate(request);
        if (!results.IsValid)
        {
            return Response<CreateProductResponse>
                .UnprocessableEntity("Invalid request",
                    results.Errors
                        .Select(x => new ResponseError(x.PropertyName, x.ErrorMessage))
                        .ToArray());
        }

        var canAddProduct = await CanAddProductAsync(request);
        if (canAddProduct.IsFailed)
        {
            return Response<CreateProductResponse>
                .BadRequest(canAddProduct.Reasons[0].Message,
                    results.Errors
                        .Select(x => new ResponseError(x.PropertyName, x.ErrorMessage))
                        .ToArray());
        }

        var addProduct = await AddProductAsync(request);
        if (addProduct.IsFailed)
        {
            return Response<CreateProductResponse>
                .InternalServerError(addProduct.Reasons[0].Message,
                    results.Errors
                        .Select(x => new ResponseError(x.PropertyName, x.ErrorMessage))
                        .ToArray());
        }

        return Response<CreateProductResponse>
            .Created(new(addProduct.Value.Id.ToString(), addProduct.Value.Active, addProduct.Value.CreateAt),
                "The product was created successfully");
    }

    private Task<Result<(Guid Id, bool Active, DateTime CreateAt)>> AddProductAsync(CreateProductRequest request)
    {
        throw new NotImplementedException();
    }

    private Task<Result> CanAddProductAsync(CreateProductRequest request)
    {
        throw new NotImplementedException();
    }
}