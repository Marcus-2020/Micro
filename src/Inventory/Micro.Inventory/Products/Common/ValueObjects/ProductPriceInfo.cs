namespace Micro.Inventory.Products.Common.ValueObjects;

internal record struct ProductPriceInfo(
    decimal CostPrice,
    decimal ProfitMargin,
    decimal SellingPrice
)
{
    public void SetCost(decimal cost, bool changeSalePrice = true)
    {
        if (cost < 0) return;
        CostPrice = cost;
       
        if (!changeSalePrice) return;
        SellingPrice = CostPrice > 0 ? CostPrice * (ProfitMargin / 100) : 0;
    }
    
    public void SetProfitMargin(decimal profitMargin, bool changeSalePrice = true)
    {
        if (profitMargin < 0) return;
        ProfitMargin = profitMargin;
       
        if (!changeSalePrice) return;
        SellingPrice = CostPrice > 0 ? CostPrice * (ProfitMargin / 100) : 0;
    }
    
    public void SetSellingPrice(decimal price, bool changeProfitMargin = true)
    {
        if (price < 0) return;
        SellingPrice = price;
       
        if (!changeProfitMargin) return;
        ProfitMargin = CostPrice > 0 ? (SellingPrice / CostPrice) * 100 : 0;
    }
}