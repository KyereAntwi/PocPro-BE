namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductDetails;

public class GetProductDetailsEndpoint(IShopDbContext shopDbContext) 
    : Endpoint<GetProductDetailsRequest, BaseResponse<GetProductDetailsResponse>>
{
    public override void Configure()
    {
        Get("/products/{ProductId}");
    }

    public override async Task HandleAsync(GetProductDetailsRequest req, CancellationToken ct)
    {
        var productWithStocks = await shopDbContext
            .Products
            .Where(p => p.Id == ProductId.Of(req.ProductId))
            .Select(p => new
            {
                Product = new GetProductDetailsResponseItem(
                    p.Id.Value,
                    p.Name,
                    p.PhotoUrl ?? string.Empty,
                    p.CreatedAt,
                    p.UpdatedAt),
                Stocks = p.Stocks.Select(s => new StockItem(
                    s.Id.Value,
                    s.SupplierId,
                    s.QuantityPurchased,
                    s.QuantityLeftInStock,
                    s.CostPerPrice,
                    s.SellingPerPrice,
                    s.TaxRate,
                    s.ExpiresAt)
                ).ToArray()
            })
            .FirstOrDefaultAsync(ct);
        
        if (productWithStocks is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(new BaseResponse<GetProductDetailsResponse>("Product details fetched successfully", true)
        {
            Data = new GetProductDetailsResponse(productWithStocks.Product, productWithStocks.Stocks)
        }, ct);
    }
}