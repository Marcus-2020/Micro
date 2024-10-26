using System.Text.Json.Serialization;

namespace Micro.Inventory.Contracts.Products.CreateProduct;

public record struct CreateProductPriceInfo(
    [property: JsonPropertyName("costPrice")]
    decimal CostPrice,
    [property: JsonPropertyName("profitMargin")]
    decimal ProfitMargin,
    [property: JsonPropertyName("sellingPrice")]
    decimal SellingPrice);