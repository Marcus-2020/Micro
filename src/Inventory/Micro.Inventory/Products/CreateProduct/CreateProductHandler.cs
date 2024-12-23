using System.Diagnostics;
using FluentResults;
using FluentResults.Extensions;
using FluentValidation;
using Micro.Core.Common.Data;
using Micro.Core.Common.Errors;
using Micro.Core.Common.Extensions;
using Micro.Core.Common.Handlers;
using Micro.Core.Common.Infra.Messaging;
using Micro.Core.Common.Responses;
using Micro.Inventory.Contracts.Products.Common.Events;
using Micro.Inventory.Contracts.Products.CreateProduct;
using Micro.Inventory.Products.Common.Data;
using Micro.Inventory.Products.Common.Entities;
using Micro.Inventory.Products.Common.Extensions;
using Micro.Inventory.Products.Common.Messaging;
using Serilog;

namespace Micro.Inventory.Products.CreateProduct;

public class CreateProductHandler : Handler<CreateProductRequest, Response<CreateProductResponse>>, ICreateProductHandler
{
    protected override IValidator<CreateProductRequest> _validator { get; }
    private readonly IDataContextFactory _dataContextFactory;
    private readonly IProductRepository _productRepository;
    private readonly IProductMessageProducer _messageProducer;

    private readonly ILogger _logger;

    public CreateProductHandler(
        IValidator<CreateProductRequest> validator, 
        IDataContextFactory dataContextFactory,
        IProductRepository productRepository, 
        IProductMessageProducer messageProducer)
    {
        _validator = validator;
        _dataContextFactory = dataContextFactory;
        _productRepository = productRepository;
        _messageProducer = messageProducer;

        _logger = Log.Logger.ForContext<CreateProductHandler>();
    }

    public override async Task<Response<CreateProductResponse>> HandleAsync(CreateProductRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var logger = _logger.ForContext("method", nameof(HandleAsync));
        
        logger.Information("Starting inserting a new product at {Timestamp}", DateTime.UtcNow);

        await using var dataContext = _dataContextFactory.CreateDataContext();

        var result = await InitializeTransactionAsync(new CreateProductResult(logger, dataContext, request, stopwatch))
            .Bind(ValidateRequestAsync)
            .Bind(MapProductAsync)
            .Bind(CanAddProductAsync)
            .Bind(AddProductAsync);

        if (result.IsFailed) return Fail(logger, stopwatch, result);

        var product = result.Value.Product;

        await dataContext.CommitAsync();

        PublishProductCreatedEvent(logger, product!);

        logger.Information("Finishing inserting the new product {CategoryId} at {Timestamp} in {ElapsedMilliseconds}ms",
            product!.Id.ToString(), DateTime.UtcNow, stopwatch.ElapsedMilliseconds);

        return Response<CreateProductResponse>
            .Created(new(product!.Id.ToString(), product.IsActive, product.CreatedAt),
                "Product created successfully");
    }
    
    private static async Task<Result<CreateProductResult>> InitializeTransactionAsync(CreateProductResult result)
    {
        var logger = result.Logger.ForContext("method", nameof(InitializeTransactionAsync));
        
        var dcInit = await result.DataContext.BeginTransactionAsync();
        
        if (dcInit.IsSuccess) return result;
        
        logger.Error(dcInit.GetFirstException(),
            "An error occurred while trying to start the connection with the database {Timestamp} after {ElapsedMilliseconds}ms", 
            DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
        
        return Result.Fail(new InternalServerError("An error occurred when trying to add the product",
            dcInit.GetFirstException()));
    }
    
    private static async Task<Result<CreateProductResult>> MapProductAsync(CreateProductResult result)
    {
        var setProduct = result.SetProduct();
        if (setProduct.IsFailed) 
            return Result.Fail<CreateProductResult>(
                new InternalServerError("Failed to map the request to a product", null));
        return result;
    }

    private async Task<Result<CreateProductResult>> CanAddProductAsync(CreateProductResult result)
    {
        var logger = GetMethodLogger(result.Logger, nameof(CanAddProductAsync));
        
        var existingProductResult = await _productRepository.GetByNameOrSku(result.DataContext, result.Product!.Sku, result.Product!.Name);
        if (existingProductResult.IsFailed)
        {
            logger.Error(existingProductResult.GetFirstException(),
                "An error occurred while trying to check if the product already exists at {Timestamp} after {ElapsedMilliseconds}ms",
                DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new InternalServerError(
                "An error occurred when trying to check if the product already exists",
                existingProductResult.GetFirstException()));
        }

        var productsFound = existingProductResult.Value.ToList();
        
        if (productsFound.Count == 0) return result;

        bool nameExists = false;
        bool skuExists = false;
        foreach (var pf in productsFound)
        {
            if (!nameExists && pf.Name == result.Product.Name) nameExists = true;
            if (!skuExists && pf.Sku == result.Product.Sku) skuExists = true;
            if (nameExists && skuExists) break;
        }

        List<ResponseError> errors = [];
        if (nameExists) errors.Add(new ResponseError(nameof(result.Product.Name), "This product name is already in use"));
        if (skuExists) errors.Add(new ResponseError(nameof(result.Product.Sku), "This SKU is already in use"));
        
        logger.ForContext("exists", new {nameExists, skuExists}, true)
            .Warning("The name ({ProductName}) or SKU ({ProductSku}) is already in use, {Timestamp} - {ElapsedMilliseconds}ms",
            result.Request.Name, result.Request.Sku, DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
        
        return Result.Fail(new ValidationError("A similar product already exist with the same name or SKU", errors));
    }
    
    private async Task<Result<CreateProductResult>> AddProductAsync(CreateProductResult result)
    {
        var logger = GetMethodLogger(result.Logger, nameof(AddProductAsync));
        
        var addResult = await _productRepository.AddAsync(result.DataContext, result.Product!.ToProductDto());
        if (addResult.IsFailed)
        {
            logger.Error(addResult.GetFirstException(),
                "An error occurred while trying to add the product to the database at {Timestamp} after {ElapsedMilliseconds}ms",
                DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new InternalServerError(
                "An error occurred when trying to add the product to the database",
                addResult.GetFirstException()));
        }
        
        var setProductId = result.Product!.SetId(addResult.Value.Id);
        if (!setProductId.IsSuccess)
        {
            logger.ForContext("ErrorCode", setProductId.Error.Code)
                .ForContext("ErrorMessage", setProductId.Error.Message)
                .Error("An error occurred while trying to set up the product id after database insertion at {Timestamp} after {ElapsedMilliseconds}ms",
                    DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new InternalServerError(
                "An error occurred when trying to add the product to the database",
                null));
        }

        
        var setCreatedAt = result.Product!.SetCreatedAt(addResult.Value.CreatedAt);
        if (!setCreatedAt.IsSuccess)
        {
            logger.ForContext("ErrorCode", setCreatedAt.Error.Code)
                .ForContext("ErrorMessage", setCreatedAt.Error.Message)
                .Error("An error occurred while trying to set up the product date of creation after database insertion at {Timestamp} after {ElapsedMilliseconds}ms",
                    DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new InternalServerError(
                "An error occurred when trying to add the product to the database",
                null));
        }

        return result;
    }
    
    private static Response<CreateProductResponse> Fail(ILogger parentLogger, Stopwatch stopwatch, Result<CreateProductResult> result)
    {
        var logger = parentLogger.ForContext("method", nameof(Fail));
        
        Response<CreateProductResponse> response;
        
        if (result.HasError<BadRequestError>())
        {
            var error = result.Errors.First(x => x is BadRequestError) as BadRequestError;
            response = Response<CreateProductResponse>.BadRequest(error!.Message, error.ValidationErrors.ToArray());
        }
        else if (result.HasError<ValidationError>())
        {
            var error = result.Errors.First(x => x is ValidationError) as ValidationError;
            response = Response<CreateProductResponse>.UnprocessableEntity(error!.Message, error.ValidationErrors.ToArray());
        }
        else
        {
            response = Response<CreateProductResponse>.InternalServerError(
                result.Errors.FirstOrDefault()?.Message ??
                "An error occurred while trying to add the new product");
        }
        
        logger.Information("Failed to insert the new product at {Timestamp} in {ElapsedMilliseconds}ms",
            DateTime.UtcNow, stopwatch.ElapsedMilliseconds);

        return response;
    }
    
    private void PublishProductCreatedEvent(ILogger parentLogger, Product product)
    {
        var logger = GetMethodLogger(parentLogger, nameof(PublishProductCreatedEvent));
        
        bool published = _messageProducer.PublishMessage(
            MessagingConstants.ProductCreated.RoutingKey,
            EventNames.ProductCreated,
            new ProductCreatedEvent(product.Id.ToString(), product.Sku, product.Name, product.Description,
                (int)product.ProductType, product.Category.Id.ToString(), product.Unit.Id.ToString(),
                product.PriceInfo.CostPrice, product.PriceInfo.ProfitMargin, product.PriceInfo.SellingPrice,
                product.CreatedAt));
        
        if (!published)
            logger.Warning(
                "An error occurred while publishing the {EventName} event at {Timestamp} for the product {ProductId}",
                EventNames.ProductCreated, DateTime.UtcNow, product.Id);
        else
            logger.Information("Published the {EventName} event at {Timestamp} for the product {ProductId}",
                EventNames.ProductCreated, DateTime.UtcNow, product.Id);
    }
}