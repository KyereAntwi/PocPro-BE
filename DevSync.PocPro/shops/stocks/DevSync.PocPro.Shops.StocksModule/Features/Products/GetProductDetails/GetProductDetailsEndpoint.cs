namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductDetails;

public class GetProductDetailsEndpoint(IShopDbContext shopDbContext) 
    : Endpoint<GetProductDetailsRequest, BaseResponse<GetProductDetailsResponseItem>>
{
    public override void Configure()
    {
        Get("/api/v1/products/{ProductId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProductDetailsRequest req, CancellationToken ct)
    {
        var productWithStocks = await shopDbContext
            .Products
            .Include(p => p.Stocks)
            .Where(p => p.Id == ProductId.Of(req.ProductId))
            .Select(p => new
            {
                Product = new GetProductDetailsResponseItem(
                    p.Id.Value,
                    p.Name,
                    p.BarcodeNumber ?? string.Empty,
                    p.PhotoUrl ?? string.Empty,
                    p.CreatedAt,
                    p.UpdatedAt,
                    p.CategoryId.Value,
                    p.Description ?? string.Empty,
                    p.LowThresholdValue)
                {
                    //ProductMedia = p.Media.Select(m => new MediaItemResponse(m.Url, m.MediaType.ToString())),
                    Price = p.CurrentSellingPrice(null)
                }
            })
            .AsNoTracking()
            .AsSplitQuery()
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