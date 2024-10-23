namespace Micro.Inventory.Products.CreateProduct.Errors.CantAddProduct;

internal class NameAlreadyExists : ICantAddProductError
{
    public string Message { get; init; }
    public Dictionary<string, object> Metadata { get; init; }

    public NameAlreadyExists(string name)
    {
        Message = $"A product with the name '{name}' already exists";
        Metadata = new Dictionary<string, object>();
    }
}