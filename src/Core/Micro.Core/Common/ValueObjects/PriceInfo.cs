namespace Micro.Core.Common.ValueObjects;

public readonly record struct PriceInfo(
    decimal Quantity,
    decimal Price,
    decimal Discount,
    decimal Taxes,
    decimal OtherExpenses
)
{
    public decimal Total() => (Quantity * Price) - Discount + Taxes + OtherExpenses;
}