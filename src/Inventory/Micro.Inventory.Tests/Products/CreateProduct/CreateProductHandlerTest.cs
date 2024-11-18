using FluentAssertions;
using FluentResults;
using FluentValidation;
using Micro.Core.Common.Data;
using Micro.Core.Common.Infra.Messaging;
using Micro.Inventory.Contracts.Products.CreateProduct;
using Micro.Inventory.Products.Common.Data;
using Micro.Inventory.Products.Common.DTOs;
using Micro.Inventory.Products.Common.Messaging;
using Micro.Inventory.Products.CreateProduct;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Micro.Inventory.Tests.Products.CreateProduct;

public class CreateProductHandlerTests
{
    private readonly IServiceProvider _serviceProvider;

    public CreateProductHandlerTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IValidator<CreateProductRequest>, CreateProductValidator>();
        
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

    private IProductRepository SetupProductRepositoryMock(Guid productId)
    {
        var productRepositoryMock = Substitute.For<IProductRepository>();
        productRepositoryMock.AddAsync(Arg.Any<IDataContext>(), Arg.Any<ProductDto>())
            .Returns(Result.Ok((productId, DateTime.UtcNow)));
        productRepositoryMock.GetByNameOrSku(Arg.Any<IDataContext>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(new List<ProductDto>());

        return productRepositoryMock;
    }

    private IProductMessageProducer SetupMessageProducerMock()
    {
        var messageProducerMock = Substitute.For<IProductMessageProducer>();
        messageProducerMock.PublishMessage(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IMessage>()).Returns(true);

        return messageProducerMock;
    }

    [Fact]
    public async Task CreateProductHandler_HandleAsync_ShouldReturnResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var dataContextMock = SetupDataContextMock();
        var dataContextFactoryMock = SetupDataContextFactoryMock(dataContextMock);
        var productRepositoryMock = SetupProductRepositoryMock(productId);
        var messageProducerMock = SetupMessageProducerMock();

        var validator = _serviceProvider.GetRequiredService<IValidator<CreateProductRequest>>();
        var sot = new CreateProductHandler(validator, dataContextFactoryMock, productRepositoryMock, messageProducerMock);

        var request = new CreateProductRequest
        {
            Sku = "ABC123",
            Name = "Test Product",
            Description = "This is a test product",
            Type = 1,
            CategoryId = Guid.NewGuid().ToString(),
            UnitId = Guid.NewGuid().ToString(),
            PriceInfo = new CreateProductPriceInfo { CostPrice = 10.00m, ProfitMargin = 0.2m, SellingPrice = 12.00m }
        };

        // Act
        var response = await sot.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(201);
        response.Message.Should().Be("Product created successfully");
        response.Data.Id.Should().Be(productId.ToString());
    }
}