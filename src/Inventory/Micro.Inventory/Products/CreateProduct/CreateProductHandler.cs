using System.Diagnostics;
using FluentResults;
using FluentValidation;
using Micro.Core.Common.Data;
using Micro.Core.Common.Extensions;
using Micro.Core.Common.Responses;
using Micro.Inventory.Products.Common.Data;
using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.CreateProduct.Errors;
using Micro.Inventory.Products.CreateProduct.Errors.CantAddProduct;
using Micro.Inventory.Products.CreateProduct.Mappers;
using Micro.Inventory.Products.CreateProduct.Requests;
using Micro.Inventory.Products.CreateProduct.Responses;
using Serilog;

namespace Micro.Inventory.Products.CreateProduct;

internal class CreateProductHandler : ICreateProductHandler
{
    private readonly IValidator<CreateProductRequest> _validator;
    private readonly IDataContextFactory _dataContextFactory;
    private readonly IProductRepository _productRepository;

    private readonly ILogger _logger;

    public CreateProductHandler(
        IValidator<CreateProductRequest> validator, 
        IDataContextFactory dataContextFactory,
        IProductRepository productRepository)
    {
        _validator = validator;
        _dataContextFactory = dataContextFactory;
        _productRepository = productRepository;

        _logger = Log.Logger.ForContext<CreateProductHandler>();
    }

    public async Task<Response<CreateProductResponse>> HandleAsync(CreateProductRequest request)
    {
        var watch = Stopwatch.StartNew();
        var logger = _logger.ForContext("method", nameof(HandleAsync));
        
        logger.Information("Starting product insertion in the inventory module at {Timestamp}", DateTime.UtcNow);

        await using var dataContext = _dataContextFactory.CreateDataContext();

        var dcInit = await dataContext.BeginTransactionAsync();
        if (dcInit.IsFailed)
        {
            logger.Error(dcInit.GetFirstException(),
                "An error occurred while trying to start the connection with the database {Timestamp} after {ElapsedMilliseconds}ms", 
                DateTime.UtcNow, watch.ElapsedMilliseconds);
            return Response<CreateProductResponse>
                .InternalServerError("An error occurred when trying to add the product", null);
        }
        
        var results = await _validator.ValidateAsync(request);
        if (!results.IsValid)
        {
            logger.ForContext("validationErrors", results.Errors.Select(x=>new{x.PropertyName, x.ErrorMessage}), true)
                .Warning("Invalid request at {Timestamp} after {ElapsedMilliseconds}ms", DateTime.UtcNow, watch.ElapsedMilliseconds);
            return Response<CreateProductResponse>
                .UnprocessableEntity("Invalid request",
                    results.Errors
                        .Select(x => new ResponseError(x.PropertyName, x.ErrorMessage))
                        .ToArray());
        }

        var product = request.ToProduct();

        var canAddProduct = await CanAddProductAsync(dataContext, product);
        if (canAddProduct.IsFailed)
        {
            logger.Error(canAddProduct.GetFirstException(),
                "An error occurred while checking if can add the product at {Timestamp} after {ElapsedMilliseconds}ms", 
                DateTime.UtcNow, watch.ElapsedMilliseconds);
            return Response<CreateProductResponse>
                .InternalServerError("An error occurred when trying to add the product", null);
        }

        if (!canAddProduct.Value)
        {
            var reasons = GetResponseErrors(canAddProduct.Reasons.OfType<ICantAddProductError>().ToArray());
            logger.ForContext("reasons", reasons)
                .Warning("Process stopped because can't add the product at {Timestamp} after {ElapsedMilliseconds}ms",
                    DateTime.UtcNow, watch.ElapsedMilliseconds);
            return Response<CreateProductResponse>
                .BadRequest(canAddProduct.Reasons[0].Message, reasons);
        }

        var addProduct = await AddProductAsync(dataContext, product);
        if (addProduct.IsFailed)
        {
            logger.Error(canAddProduct.GetFirstException(),
                "An error occurred while trying to add the product at {Timestamp} after {ElapsedMilliseconds}ms", 
                DateTime.UtcNow, watch.ElapsedMilliseconds);
            await dataContext.RollbackAsync();
            return Response<CreateProductResponse>
                .InternalServerError(addProduct.Errors[0].Message, null);
        }

        await dataContext.CommitAsync();

        return Response<CreateProductResponse>
            .Created(new(addProduct.Value.Id.ToString(), addProduct.Value.Active, addProduct.Value.CreateAt),
                "The product was created successfully");
    }

    private async Task<Result<(Guid Id, bool Active, DateTime CreateAt)>> AddProductAsync(IDataContext dataContext, Product product)
    {
        var addProduct = await _productRepository.AddAsync(dataContext, product);
        if (addProduct.IsFailed)
        {
            return Result
                .Fail(new Error("An error occurred when trying to add the product")
                    .WithMetadata(ErrorsConstants.AddProductFailed, string.Empty));
        }

        return Result.Ok();
    }

    private async Task<Result<bool>> CanAddProductAsync(IDataContext dataContext, Product product)
    {
        var canAdd = await _productRepository.GetByNameOrSku(dataContext, product.Sku, product.Name);
        if (canAdd.IsFailed)
        {
            return Result
                .Fail<bool>(new Error("An error occurred when trying to verify if could add the product")
                    .WithMetadata(ErrorsConstants.CanAddProductFailed, string.Empty));
        }

        var productsFound = canAdd.Value.ToList();
        
        if (productsFound.Count == 0) return Result.Ok(true);

        bool skuExists = false;
        bool nameExists = false;
        foreach (var pf in productsFound)
        {
            if (!skuExists && pf.Sku == product.Sku) skuExists = true;
            if (!nameExists && pf.Name == product.Name) nameExists = true;
            if (skuExists && nameExists) break;
        }

        List<ICantAddProductError> reasons = [];
        if (skuExists) reasons.Add(new SkuAlreadyExists(product.Sku));
        if (nameExists) reasons.Add(new NameAlreadyExists(product.Name));

        return Result.Ok(false).WithReasons(reasons);
    }
    
    private ResponseError[]? GetResponseErrors(ICantAddProductError[] reasons)
    {
        return reasons
            .Where(x => x is SkuAlreadyExists or NameAlreadyExists)
            .Select(x =>
        {
            return x is SkuAlreadyExists 
                ? new ResponseError(nameof(Product.Sku), x.Message) 
                : new ResponseError(nameof(Product.Name), x.Message);
        }).ToArray();
    }
}