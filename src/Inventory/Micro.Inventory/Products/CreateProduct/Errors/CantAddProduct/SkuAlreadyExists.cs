namespace Micro.Inventory.Products.CreateProduct.Errors.CantAddProduct;

internal class SkuAlreadyExists : ICantAddProductError
{
    public string Message { get; init; }
    public Dictionary<string, object> Metadata { get; init; }

    public SkuAlreadyExists(string sku)
    {
        Message = $"A product with the SKU '{sku}' already exists";
        Metadata = new Dictionary<string, object>();
    }
}