using System.Diagnostics;
using FluentResults;
using FluentResults.Extensions;
using FluentValidation;
using Micro.Core.Common.Data;
using Micro.Core.Common.Extensions;
using Micro.Core.Common.Handlers;
using Micro.Core.Common.Infra.Messaging;
using Micro.Core.Common.Responses;
using Micro.Inventory.Common.Errors;
using Micro.Inventory.Contracts.Products.Categories.Common.Events;
using Micro.Inventory.Contracts.Products.Categories.CreateCategory;
using Micro.Inventory.Products.Categories.Common.Data;
using Micro.Inventory.Products.Categories.Common.Messaging;
using Micro.Inventory.Products.Common.Entities;
using Serilog;

namespace Micro.Inventory.Products.Categories.CreateCategory;

internal class CreateCategoryHandler : Handler<CreateCategoryRequest, Response<CreateCategoryResponse>>, ICreateCategoryHandler
{
    #region Constructor and Properties

    private readonly IValidator<CreateCategoryRequest> _validator;
    private readonly IDataContextFactory _dataContextFactory;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryMessageProducer _messagingProducer;

    private readonly ILogger _logger;

    public CreateCategoryHandler(
        IValidator<CreateCategoryRequest> validator,
        IDataContextFactory dataContextFactory,
        ICategoryRepository categoryRepository,
        ICategoryMessageProducer messagingProducer)
    {
        _validator = validator;
        _dataContextFactory = dataContextFactory;
        _categoryRepository = categoryRepository;
        _messagingProducer = messagingProducer;

        _logger = Log.Logger.ForContext<CreateCategoryHandler>();
    }

    #endregion

    public override async Task<Response<CreateCategoryResponse>> HandleAsync(CreateCategoryRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var logger = GetMethodLogger(_logger, nameof(HandleAsync));
        
        logger.Information("Starting inserting a new product category at {Timestamp}", DateTime.UtcNow);

        await using var dataContext = _dataContextFactory.CreateDataContext();

        var result = await InitializeTransactionAsync(new CreateCategoryResult(logger, dataContext, request, stopwatch))
            .Bind(ValidateRequestAsync)
            .Bind(MapCategoryAsync)
            .Bind(ValidateIfNotExistsAsync)
            .Bind(AddCategoryAsync);

        if (result.IsFailed) return Fail(logger, stopwatch, result);

        var category = result.Value.Category;

        await dataContext.CommitAsync();

        PublishCategoryCreatedEvent(logger, category!);
        
        logger.Information("Finishing inserting the new product category {CategoryId} at {Timestamp} in {ElapsedMilliseconds}ms",
            category!.Id.ToString(), DateTime.UtcNow, stopwatch.ElapsedMilliseconds);

        return Response<CreateCategoryResponse>
            .Created(new(category!.Id.ToString(), category.IsActive, category.CreatedAt),
                "The product category was created successfully");
    }

    private static async Task<Result<CreateCategoryResult>> InitializeTransactionAsync(CreateCategoryResult result)
    {
        var logger = result.Logger.ForContext("method", nameof(InitializeTransactionAsync));
        
        var dcInit = await result.DataContext.BeginTransactionAsync();
        
        if (dcInit.IsSuccess) return result;
        
        logger.Error(dcInit.GetFirstException(),
            "An error occurred while trying to start the connection with the database {Timestamp} after {ElapsedMilliseconds}ms", 
            DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
        
        return Result.Fail(new InternalServerError("An error occurred when trying to add the product category",
            dcInit.GetFirstException()));
    }

    private async Task<Result<CreateCategoryResult>> ValidateRequestAsync(CreateCategoryResult result)
    {
        var logger = GetMethodLogger(result.Logger, nameof(ValidateRequestAsync));
        
        var results = await _validator.ValidateAsync(result.Request);
        if (results.IsValid) return result;
        
        logger.ForContext("validationErrors", results.Errors.Select(x=>new{x.PropertyName, x.ErrorMessage}), true)
            .Warning("Invalid request at {Timestamp} after {ElapsedMilliseconds}ms", DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
        
        return Result.Fail(new BadRequestError(
            "Invalid request",
            results.Errors
                .Select(x => new ResponseError(x.PropertyName, x.ErrorMessage))
                .ToList()));
    }

    private static async Task<Result<CreateCategoryResult>> MapCategoryAsync(CreateCategoryResult result)
    {
        var setCategory = result.SetCategory();
        if (setCategory.IsFailed) 
            return Result.Fail<CreateCategoryResult>(
                    new InternalServerError("Failed to map the request to a product category", null));
        return result;
    }

    private async Task<Result<CreateCategoryResult>> ValidateIfNotExistsAsync(CreateCategoryResult result)
    {
        var logger = GetMethodLogger(result.Logger, nameof(ValidateIfNotExistsAsync));
        
        var existingCategoryResult = await _categoryRepository.GetByNameAsync(result.DataContext, result.Request.Name);
        if (existingCategoryResult.IsFailed)
        {
            logger.Error(existingCategoryResult.GetFirstException(),
                "An error occurred while trying to check if the category already exists at {Timestamp} after {ElapsedMilliseconds}ms",
                DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new InternalServerError(
                "An error occurred when trying to check if the category already exists",
                existingCategoryResult.GetFirstException()));
        }

        if (existingCategoryResult.Value.Any())
        {
            logger.Warning(
                "Category with name {CategoryName} already exists at {Timestamp} after {ElapsedMilliseconds}ms",
                result.Request.Name, DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new ValidationError(
                "Category with the same name already exists",
                [new ResponseError("Name", "This name is already in use")]));
        }

        return result;
    }

    private async Task<Result<CreateCategoryResult>> AddCategoryAsync(CreateCategoryResult result)
    {
        var logger = GetMethodLogger(result.Logger, nameof(AddCategoryAsync));
        
        var addResult = await _categoryRepository.AddAsync(result.DataContext, result.Category!);
        if (addResult.IsFailed)
        {
            logger.Error(addResult.GetFirstException(),
                "An error occurred while trying to add the category to the database at {Timestamp} after {ElapsedMilliseconds}ms",
                DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new InternalServerError(
                "An error occurred when trying to add the category to the database",
                addResult.GetFirstException()));
        }

        var setCreatedAt = result.Category!.SetCreatedAt(addResult.Value.CreatedAt);
        if (!setCreatedAt.IsSuccess)
        {
            logger.ForContext("ErrorCode", setCreatedAt.Error.Code)
                .ForContext("ErrorMessage", setCreatedAt.Error.Message)
                .Error("An error occurred while trying to set up the category date of creation after database insertion at {Timestamp} after {ElapsedMilliseconds}ms",
                DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new InternalServerError(
                "An error occurred when trying to add the category to the database",
                null));
        }

        return result;
    }

    private static Response<CreateCategoryResponse> Fail(ILogger parentLogger, Stopwatch stopwatch, Result<CreateCategoryResult> result)
    {
        var logger = parentLogger.ForContext("method", nameof(Fail));
        
        Response<CreateCategoryResponse> response;
        
        if (result.HasError<InternalServerError>())
        {
            var error = result.Errors.First(x => x is InternalServerError);
            response = Response<CreateCategoryResponse>.InternalServerError(error.Message);
        }
        else if (result.HasError<ValidationError>())
        {
            var error = result.Errors.First(x => x is ValidationError) as ValidationError;
            response = Response<CreateCategoryResponse>.UnprocessableEntity(error!.Message, error.ValidationErrors.ToArray());
        }
        else
        {
            response = Response<CreateCategoryResponse>.InternalServerError(
                result.Errors.FirstOrDefault()?.Message ??
                "An error occurred while trying to add the new category");
        }
        
        logger.Information("Failed to insert the new product category at {Timestamp} in {ElapsedMilliseconds}ms",
            DateTime.UtcNow, stopwatch.ElapsedMilliseconds);

        return response;
    }

    private void PublishCategoryCreatedEvent(ILogger parentLogger, ProductCategory category)
    {
        var logger = GetMethodLogger(parentLogger, nameof(PublishCategoryCreatedEvent));
        
        bool published = _messagingProducer.PublishMessage(MessagingConstants.CategoryCreated.RoutingKey,
            EventNames.CategoryCreated,
            new CategoryCreatedEvent(category!.Id.ToString(), category.Name, category.CreatedAt));
        
        if (!published)
            logger.Warning(
                "An error occurred while publishing the {EventName}  event at {Timestamp} for the category {CategoryId}",
                EventNames.CategoryCreated, DateTime.UtcNow, category.Id);
        else
            logger.Information("Published the {EventName} event at {Timestamp} for the category {CategoryId}",
                EventNames.CategoryCreated, DateTime.UtcNow, category.Id);
    }
}