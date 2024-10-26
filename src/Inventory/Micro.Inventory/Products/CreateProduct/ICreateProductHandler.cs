using Micro.Core.Common.Handlers;
using Micro.Core.Common.Responses;
using Micro.Inventory.Contracts.Products.CreateProduct;

namespace Micro.Inventory.Products.CreateProduct;

public interface ICreateProductHandler : IHandler<CreateProductRequest, Response<CreateProductResponse>> {}