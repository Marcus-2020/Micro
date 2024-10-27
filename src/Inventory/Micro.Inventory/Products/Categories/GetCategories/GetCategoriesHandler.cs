using System.Diagnostics;
using FluentResults;
using FluentResults.Extensions;
using FluentValidation;
using Micro.Core.Common.Data;
using Micro.Core.Common.Errors;
using Micro.Core.Common.Extensions;
using Micro.Core.Common.Handlers;
using Micro.Core.Common.Responses;
using Micro.Inventory.Contracts.Products.Categories.GetCategories;
using Micro.Inventory.Products.Categories.Common.Data;
using Serilog;

namespace Micro.Inventory.Products.Categories.GetCategories;

internal class GetCategoriesHandler : Handler<GetCategoriesRequest, Response<GetCategoriesResponse>>, IGetCategoriesHandler
{
    protected override IValidator<GetCategoriesRequest> _validator { get; }
    private readonly IDataContextFactory _dataContextFactory;
    private readonly ICategoryRepository _categoryRepository;
    
    private readonly ILogger _logger;

    public GetCategoriesHandler(
        IDataContextFactory dataContextFactory,
        ICategoryRepository categoryRepository,
        IValidator<GetCategoriesRequest> validator)
    {
        _dataContextFactory = dataContextFactory;
        _categoryRepository = categoryRepository;
        _validator = validator;

        _logger = Log.Logger.ForContext<GetCategoriesHandler>();
    }

    public override async Task<Response<GetCategoriesResponse>> HandleAsync(GetCategoriesRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var logger = GetMethodLogger(_logger, nameof(HandleAsync));
        
        logger.Information("Starting inserting a new product category at {Timestamp}", DateTime.UtcNow);

        await using var dataContext = _dataContextFactory.CreateDataContext();

        var result = await InitializeTransactionAsync(new GetCategoriesResult(logger, dataContext, request, stopwatch))
            .Bind(ValidateRequestAsync)
            .Bind(CountCategoriesAsync)
            .Bind(GetCategoriesAsync);
        
        if (result.IsFailed) return Fail(logger, stopwatch, result);

        await dataContext.CommitAsync();
        
        logger.Information("Finishing recovering {Count} product categories at {Timestamp} in {ElapsedMilliseconds}ms",
            result.Value.Categories.Count, DateTime.UtcNow, stopwatch.ElapsedMilliseconds);

        return Response<GetCategoriesResponse>
            .Ok(new(result.Value.MapCategoriesToResponseItems(), result.Value.Count, request.Skip, request.Take),
                "Product categories recovered successfully");
    }
    
    private static async Task<Result<GetCategoriesResult>> InitializeTransactionAsync(GetCategoriesResult result)
    {
        var logger = result.Logger.ForContext("method", nameof(InitializeTransactionAsync));
        
        var dcInit = await result.DataContext.BeginTransactionAsync();
        
        if (dcInit.IsSuccess) return result;
        
        logger.Error(dcInit.GetFirstException(),
            "An error occurred while trying to start the connection with the database {Timestamp} after {ElapsedMilliseconds}ms", 
            DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
        
        return Result.Fail(new InternalServerError("An error occurred when trying to recover product categories",
            dcInit.GetFirstException()));
    }
    
    private async Task<Result<GetCategoriesResult>> CountCategoriesAsync(GetCategoriesResult result)
    {
        var logger = GetMethodLogger(result.Logger, nameof(CountCategoriesAsync));

        var countResult = await _categoryRepository.CountAsync(result.DataContext);
        if (countResult.IsFailed)
        {
            logger.Error(countResult.GetFirstException(),
                "An error occurred while counting the product categories in the database at {Timestamp} after {ElapsedMilliseconds}ms",
                DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new InternalServerError(
                "An error occurred while counting the product categories in the database",
                countResult.GetFirstException()));
        }

        result.Count = countResult.Value;
        return result;
    }

    private async Task<Result<GetCategoriesResult>> GetCategoriesAsync(GetCategoriesResult result)
    {
        var logger = GetMethodLogger(result.Logger, nameof(GetCategoriesAsync));
        
        var getAllResult = await _categoryRepository.GetAllAsync(result.DataContext, result.Request.Skip, result.Request.Take);
        if (getAllResult.IsFailed)
        {
            logger.Error(getAllResult.GetFirstException(),
                "An error occurred while trying to recover product categories from the database at {Timestamp} after {ElapsedMilliseconds}ms",
                DateTime.UtcNow, result.Stopwatch.ElapsedMilliseconds);
            return Result.Fail(new InternalServerError(
                "An error occurred while trying to recover product categories from the database",
                getAllResult.GetFirstException()));
        }

        result.Categories = getAllResult.Value.ToList();
        return result;
    }
    
    private static Response<GetCategoriesResponse> Fail(ILogger parentLogger, Stopwatch stopwatch, Result<GetCategoriesResult> result)
    {
        var logger = parentLogger.ForContext("method", nameof(Fail));
        
        Response<GetCategoriesResponse> response;
        
        if (result.HasError<InternalServerError>())
        {
            var error = result.Errors.First(x => x is InternalServerError);
            response = Response<GetCategoriesResponse>.InternalServerError(error.Message);
        }
        else if (result.HasError<ValidationError>())
        {
            var error = result.Errors.First(x => x is ValidationError) as ValidationError;
            response = Response<GetCategoriesResponse>.UnprocessableEntity(error!.Message, error.ValidationErrors.ToArray());
        }
        else
        {
            response = Response<GetCategoriesResponse>.InternalServerError(
                result.Errors.FirstOrDefault()?.Message ??
                "An error occurred while recovering product categories");
        }
        
        logger.Information("Failed to recover product categories at {Timestamp} in {ElapsedMilliseconds}ms",
            DateTime.UtcNow, stopwatch.ElapsedMilliseconds);

        return response;
    }
}