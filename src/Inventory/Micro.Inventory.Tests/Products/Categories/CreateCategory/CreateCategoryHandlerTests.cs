using FluentAssertions;
using FluentResults;
using FluentValidation;
using Micro.Core.Common.Data;
using Micro.Core.Common.Infra.Messaging;
using Micro.Inventory.Contracts.Products.Categories.CreateCategory;
using Micro.Inventory.Products.Categories.Common.Data;
using Micro.Inventory.Products.Categories.Common.DTOs;
using Micro.Inventory.Products.Categories.Common.Messaging;
using Micro.Inventory.Products.Categories.CreateCategory;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Micro.Inventory.Tests.Products.Categories.CreateCategory;

public class CreateCategoryHandlerTests
{
    private readonly IServiceProvider _serviceProvider;

    public CreateCategoryHandlerTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IValidator<CreateCategoryRequest>, CreateCategoryValidator>();
        
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
        categoryRepositoryMock.AddAsync(Arg.Any<IDataContext>(), Arg.Any<CategoryDto>())
            .Returns(Result.Ok((productId, DateTime.UtcNow)));
        categoryRepositoryMock.GetByNameAsync(Arg.Any<IDataContext>(), Arg.Any<string>())
            .Returns(new List<CategoryDto>());

        return categoryRepositoryMock;
    }

    private ICategoryMessageProducer SetupMessageProducerMock()
    {
        var messageProducerMock = Substitute.For<ICategoryMessageProducer>();
        messageProducerMock.PublishMessage(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IMessage>()).Returns(true);

        return messageProducerMock;
    }

    [Fact]
    public async Task CreateCategoryHandler_HandleAsync_ShouldReturnResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var dataContextMock = SetupDataContextMock();
        var dataContextFactoryMock = SetupDataContextFactoryMock(dataContextMock);
        var productRepositoryMock = SetupCategoryRepositoryMock(productId);
        var messageProducerMock = SetupMessageProducerMock();

        var validator = _serviceProvider.GetRequiredService<IValidator<CreateCategoryRequest>>();
        var sot = new CreateCategoryHandler(validator, dataContextFactoryMock, productRepositoryMock, messageProducerMock);

        var request = new CreateCategoryRequest("Test Category", "This is a test category", true);

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(201);
        response.Message.Should().Be("Category created successfully");
        response.Data.Should().NotBeNull();
        response.Data!.Id.Should().Be(productId.ToString());
    }
    
    [Fact]
    public async Task CreateCategoryHandler_HandleAsync_ShouldFailValidation()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var dataContextMock = SetupDataContextMock();
        var dataContextFactoryMock = SetupDataContextFactoryMock(dataContextMock);
        var productRepositoryMock = SetupCategoryRepositoryMock(productId);
        var messageProducerMock = SetupMessageProducerMock();

        var validator = _serviceProvider.GetRequiredService<IValidator<CreateCategoryRequest>>();
        var sot = new CreateCategoryHandler(validator, dataContextFactoryMock, productRepositoryMock, messageProducerMock);

        await CreateCategory_NullName(sot);
        await CreateCategory_EmptyName(sot);
        await CreateCategory_NullDescription(sot);
        await CreateCategory_EmptyDescription(sot);
    }

    private async Task CreateCategory_NullName(ICreateCategoryHandler sot)
    {
        var request = new CreateCategoryRequest(null, "This is a test category", true);

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }
    
    private async Task CreateCategory_EmptyName(ICreateCategoryHandler sot)
    {
        var request = new CreateCategoryRequest("", "This is a test category", true);

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }
    
    private async Task CreateCategory_NullDescription(ICreateCategoryHandler sot)
    {
        var request = new CreateCategoryRequest("Category 1", null, true);

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }
    
    private async Task CreateCategory_EmptyDescription(ICreateCategoryHandler sot)
    {
        var request = new CreateCategoryRequest("Category 1", "", true);

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(400);
    }
}