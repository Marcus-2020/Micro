namespace Micro.Inventory.Products.Common.ValueObjects;

internal record struct ProductPriceInfo(
    decimal Cost,
    decimal ProfitMargin,
    decimal SellingPrice
)
{
    public void SetCost(decimal cost, bool changeSalePrice = true)
    {
        if (cost < 0) return;
        Cost = cost;
       
        if (!changeSalePrice) return;
        SellingPrice = Cost > 0 ? Cost * (ProfitMargin / 100) : 0;
    }
    
    public void SetProfitMargin(decimal profitMargin, bool changeSalePrice = true)
    {
        if (profitMargin < 0) return;
        ProfitMargin = profitMargin;
       
        if (!changeSalePrice) return;
        SellingPrice = Cost > 0 ? Cost * (ProfitMargin / 100) : 0;
    }
    
    public void SetSellingPrice(decimal price, bool changeProfitMargin = true)
    {
        if (price < 0) return;
        SellingPrice = price;
       
        if (!changeProfitMargin) return;
        ProfitMargin = Cost > 0 ? (SellingPrice / Cost) * 100 : 0;
    }
}