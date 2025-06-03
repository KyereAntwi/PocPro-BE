namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductDetails;

public class GetProductDetailsEndpoint(IShopDbContext shopDbContext) 
    : Endpoint<GetProductDetailsRequest, BaseResponse<GetProductDetailsResponseItem>>
{
    public override void Configure()
    {
        Get("/api/v1/products/{ProductId}");
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
                    p.UpdatedAt)
                {
                    StocksIds = p.Stocks.Select(s => s.Id.Value),
                    CategoryId = p.CategoryId.Value,
                }
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
        
        if (productWithStocks is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(new BaseResponse<GetProductDetailsResponseItem>("Product details fetched successfully", true)
        {
            Data = productWithStocks.Product
        }, ct);
    }
}