using Micro.Core.Common.Handlers;
using Micro.Core.Common.Responses;
using Micro.Inventory.Products.CreateProduct.Requests;
using Micro.Inventory.Products.CreateProduct.Responses;

namespace Micro.Inventory.Products.CreateProduct;

public interface ICreateProductHandler : IHandler<CreateProductRequest, Response<CreateProductResponse>> {}