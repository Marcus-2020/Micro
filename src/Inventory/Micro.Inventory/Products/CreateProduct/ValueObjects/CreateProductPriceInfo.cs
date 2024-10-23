using System.Text.Json.Serialization;

namespace Micro.Inventory.Products.CreateProduct.ValueObjects;

public record struct CreateProductPriceInfo(
    [property: JsonPropertyName("cost")]
    decimal Cost,
    [property: JsonPropertyName("profitMargin")]
    decimal ProfitMargin,
    [property: JsonPropertyName("salePrice")]
    decimal SalePrice);