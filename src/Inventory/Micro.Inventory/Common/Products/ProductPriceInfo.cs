namespace Micro.Inventory.Common.Products;

internal record struct ProductPriceInfo(
    decimal Cost,
    decimal ProfitMargin,
    decimal SalePrice
)
{
    public void SetCost(decimal cost, bool changeSalePrice = true)
    {
        if (cost < 0) return;
        Cost = cost;
       
        if (!changeSalePrice) return;
        SalePrice = Cost > 0 ? Cost * (ProfitMargin / 100) : 0;
    }
    
    public void SetProfitMargin(decimal profitMargin, bool changeSalePrice = true)
    {
        if (profitMargin < 0) return;
        ProfitMargin = profitMargin;
       
        if (!changeSalePrice) return;
        SalePrice = Cost > 0 ? Cost * (ProfitMargin / 100) : 0;
    }
    
    public void SetSalePrice(decimal price, bool changeProfitMargin = true)
    {
        if (price < 0) return;
        SalePrice = price;
       
        if (!changeProfitMargin) return;
        ProfitMargin = Cost > 0 ? (SalePrice / Cost) * 100 : 0;
    }
}