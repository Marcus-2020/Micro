using System.Text.Json.Serialization;
using Micro.Inventory.Products.CreateProduct.ValueObjects;

namespace Micro.Inventory.Products.CreateProduct.Requests;

public record struct CreateProductRequest(
    [property: JsonPropertyName("sku")]
    string Sku,
    [property: JsonPropertyName("name")]
    string Name, 
    [property: JsonPropertyName("description")]
    string Description,
    [property: JsonPropertyName("type")]
    int Type,
    [property: JsonPropertyName("categoryId")]
    string CategoryId,
    [property: JsonPropertyName("unitId")]
    string UnitId,
    [property: JsonPropertyName("priceInfo")]
    CreateProductPriceInfo PriceInfo);