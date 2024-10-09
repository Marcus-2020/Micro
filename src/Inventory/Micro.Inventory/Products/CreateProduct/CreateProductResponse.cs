using System.Text.Json.Serialization;

namespace Micro.Inventory.Products.CreateProduct;

public record struct CreateProductResponse(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("active")]
    bool Active,
    [property: JsonPropertyName("createdAt")]
    DateTime CreateAt);