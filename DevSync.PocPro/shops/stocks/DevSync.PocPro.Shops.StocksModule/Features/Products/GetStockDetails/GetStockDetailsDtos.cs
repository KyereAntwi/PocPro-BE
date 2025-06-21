namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetStockDetails;

public record GetStockDetailsRequest([FromRoute] Guid StockId, [FromRoute] Guid ProductId);

public record GetStockDetailsResponse
{
    public StockItem Stock { get; set; } = default!;
}