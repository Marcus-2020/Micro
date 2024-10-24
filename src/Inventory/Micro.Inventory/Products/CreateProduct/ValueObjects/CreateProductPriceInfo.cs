using System.Text.Json.Serialization;

namespace Micro.Inventory.Products.CreateProduct.ValueObjects;

public record struct CreateProductPriceInfo(
    [property: JsonPropertyName("costPrice")]
    decimal CostPrice,
    [property: JsonPropertyName("profitMargin")]
    decimal ProfitMargin,
    [property: JsonPropertyName("sellingPrice")]
    decimal SellingPrice);