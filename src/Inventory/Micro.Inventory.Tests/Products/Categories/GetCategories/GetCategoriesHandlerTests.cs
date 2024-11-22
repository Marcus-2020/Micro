using FluentAssertions;
using FluentResults;
using FluentValidation;
using Micro.Core.Common.Data;
using Micro.Core.Common.Infra.Messaging;
using Micro.Inventory.Contracts.Products.Categories.GetCategories;
using Micro.Inventory.Products.Categories.Common.Data;
using Micro.Inventory.Products.Categories.Common.DTOs;
using Micro.Inventory.Products.Categories.Common.Messaging;
using Micro.Inventory.Products.Categories.GetCategories;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Micro.Inventory.Tests.Products.Categories.GetCategories;

public class GetCategoriesHandlerTests
{
    private readonly IServiceProvider _serviceProvider;

    public GetCategoriesHandlerTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IValidator<GetCategoriesRequest>, GetCategoriesValidator>();
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    private IDataContext SetupDataContextMock()
    {
        var dataContextMock = Substitute.For<IDataContext>();
        dataContextMock.BeginTransactionAsync().Returns(Result.Ok());
        dataContextMock.CommitAsync().Returns(Result.Ok());
        dataContextMock.RollbackAsync().Returns(Result.Ok());
        dataContextMock.FinallyAsync().Returns(Result.Ok());

        return dataContextMock;
    }

    private IDataContextFactory SetupDataContextFactoryMock(IDataContext dataContextMock)
    {
        var dataContextFactoryMock = Substitute.For<IDataContextFactory>();
        dataContextFactoryMock.CreateDataContext().Returns(dataContextMock);

        return dataContextFactoryMock;
    }

    private ICategoryRepository SetupCategoryRepositoryMock(Guid productId)
    {
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        categoryRepositoryMock.CountAsync(Arg.Any<IDataContext>())
            .Returns(1);
        
        var cat = new CategoryDto(Guid.NewGuid(), "Category 1", "Test category 1", DateTime.Now, null, null, true);
        categoryRepositoryMock.GetAllAsync(Arg.Any<IDataContext>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new List<CategoryDto>{ cat });

        return categoryRepositoryMock;
    }

    [Fact]
    public async Task GetCategoriesHandler_HandleAsync_ShouldReturnResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var dataContextMock = SetupDataContextMock();
        var dataContextFactoryMock = SetupDataContextFactoryMock(dataContextMock);
        var productRepositoryMock = SetupCategoryRepositoryMock(productId);

        var validator = _serviceProvider.GetRequiredService<IValidator<GetCategoriesRequest>>();
        var sot = new GetCategoriesHandler(dataContextFactoryMock, productRepositoryMock, validator);

        var request = new GetCategoriesRequest(0, 1);

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(200);
        response.Message.Should().Be("Product categories recovered successfully");
        response.Data.Should().NotBeNull();
        response.Data!.Count.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task GetCategoriesHandler_HandleAsync_ShouldFailValidation()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var dataContextMock = SetupDataContextMock();
        var dataContextFactoryMock = SetupDataContextFactoryMock(dataContextMock);
        var productRepositoryMock = SetupCategoryRepositoryMock(productId);

        var validator = _serviceProvider.GetRequiredService<IValidator<GetCategoriesRequest>>();
        var sot = new GetCategoriesHandler(dataContextFactoryMock, productRepositoryMock, validator);

        await GetCategories_NegativeSkip(sot);
        await GetCategories_NegativeTake(sot);
        await GetCategories_TakeBiggerThan100(sot);
    }

    private async Task GetCategories_NegativeSkip(IGetCategoriesHandler sot)
    {
        // Arrange
        var request = new GetCategoriesRequest(-1, 1);

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }
    
    private async Task GetCategories_NegativeTake(IGetCategoriesHandler sot)
    {
        // Arrange
        var request = new GetCategoriesRequest(0, -1);

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }
    
    private async Task GetCategories_TakeBiggerThan100(IGetCategoriesHandler sot)
    {
        // Arrange
        var request = new GetCategoriesRequest(0, 101);

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }
}