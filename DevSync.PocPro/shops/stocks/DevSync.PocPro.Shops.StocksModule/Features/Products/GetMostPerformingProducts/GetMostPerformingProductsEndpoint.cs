namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetMostPerformingProducts;

public class GetMostPerformingProductsEndpoint(
    IShopDbContext shopDbContext,
    IPosServices posServices,
    IOrdersServices ordersServices) 
    : Endpoint<GetMostPerformingProductsRequest, BaseResponse<IEnumerable<GetProductsResponseItem>>>
{
    public override void Configure()
    {
        Get("/api/v1/products/deprecated/most-performing");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetMostPerformingProductsRequest req, CancellationToken ct)
    {
        List<GetProductsResponseItem> products = [];

        var productIds = await ordersServices.GetMostPerformingProductsAsync(
            req.Size,
            req.ForOnlineSite,
            ct);
        
        var posIds = await posServices.GetOnlineEnabledPosIdsAsync(ct);

        var stocks = await shopDbContext
            .Stocks
            .AsNoTracking()
            .Where(x => !req.ForOnlineSite || posIds.Contains(x.PointOfSaleId))
            .ToArrayAsync(ct);

        if (productIds.Count > 0)
        {
            foreach (var stock in stocks)
            {
                var product = await shopDbContext
                    .Products
                    .Include(p => p.Stocks)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(x => x.Id == stock.ProductId && productIds.Contains(x.Id))
                    .Select(p => new GetProductsResponseItem(
                        p.Id.Value,
                        p.Name,
                        p.CurrentSellingPrice(stock.PointOfSaleId),
                        p.TotalNumberLeftOnShelf(stock.PointOfSaleId),
                        p.PhotoUrl,
                        p.CategoryId.Value,
                        p.Description ?? string.Empty,
                        p.LowThresholdValue,
                        p.BrandId != null ? p.BrandId.Value : null,
                        stock.PointOfSaleId.Value,
                        p.IsFeatured
                    ))
                    .FirstOrDefaultAsync(ct);
                
                products.Add(product!);
            }
        }
        else
        {
            foreach (var stock in stocks)
            {
                var product = await shopDbContext
                    .Products
                    .Include(p => p.Stocks)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(x => x.Id == stock.ProductId)
                    .Select(p => new GetProductsResponseItem(
                        p.Id.Value,
                        p.Name,
                        p.CurrentSellingPrice(stock.PointOfSaleId),
                        p.TotalNumberLeftOnShelf(stock.PointOfSaleId),
                        p.PhotoUrl,
                        p.CategoryId.Value,
                        p.Description ?? string.Empty,
                        p.LowThresholdValue,
                        p.BrandId != null ? p.BrandId.Value : null,
                        stock.PointOfSaleId.Value,
                        p.IsFeatured
                    ))
                    .FirstOrDefaultAsync(ct);
                
                products.Add(product!);
            }
        }

        await SendOkAsync(
            new BaseResponse<IEnumerable<GetProductsResponseItem>>("Products retrieved successfully", true)
            {
                Data = products.Take(req.Size)
            }, ct);
    }
}

public record GetMostPerformingProductsRequest(int Size = 5, bool ForOnlineSite = true);