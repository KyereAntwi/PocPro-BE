namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductsForOnlineSite;

public class GetProductsForOnlineSiteEndpoint(
    IShopDbContext shopDbContext,
    IPosServices posServices) 
    : Endpoint<GetProductsForOnlineSiteRequest, BaseResponse<PagedResponse<GetProductsResponseItem>>>
{
    public override void Configure()
    {
        Get("/api/v1/products/for-online-site");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProductsForOnlineSiteRequest req, CancellationToken ct)
    {
        var posIds = await posServices.GetOnlineEnabledPosIdsAsync(ct);

        List<Stock> stocks = [];

        stocks = await shopDbContext
            .Stocks
            .AsNoTracking()
            .Where(x => posIds.Contains(x.PointOfSaleId))
            .ToListAsync(ct);

        List<GetProductsResponseItem> products = [];

        if (stocks.Count > 0)
        {
            foreach (var stock in stocks)
            {
                var product = await shopDbContext
                    .Products
                    .Include(p => p.Stocks)
                    .Where(p => p.Id == stock.ProductId)
                    .Select(p => new GetProductsResponseItem(
                        p.Id.Value,
                        p.Name,
                        p.CurrentSellingPrice(stock.PointOfSaleId),
                        p.PhotoUrl,
                        p.CategoryId.Value,
                        p.Description ?? string.Empty,
                        p.LowThresholdValue,
                        p.BrandId != null ? p.BrandId.Value : null
                    ))
                    .AsSplitQuery()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ct);

                if (product != null)
                {
                    products.Add(product);
                }
            }
            
            products = FilterQuery(req, products).ToList();
        }
        
        var totalCount = products.Count;
        
        products = products
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize).ToList();
        
        var response = new PagedResponse<GetProductsResponseItem>(
            req.Page,
            req.PageSize,
            totalCount,
            products
        );

        await SendAsync(
            new BaseResponse<PagedResponse<GetProductsResponseItem>>("Products fetched successfully", true)
            {
                Data = response
            }, cancellation: ct);
    }

    private static IEnumerable<GetProductsResponseItem> FilterQuery(GetProductsForOnlineSiteRequest req, IEnumerable<GetProductsResponseItem> query)
    {
        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            query = query.Where(x => x.Name.Contains(req.SearchText));
        }

        if (!string.IsNullOrWhiteSpace(req.Category))
        {
            query = query.Where(x => x.CategoryId == Guid.Parse(req.Category));
        }

        if (!string.IsNullOrWhiteSpace(req.BrandIds))
        {
            var brandIds = req.BrandIds.Split(';').Select(Guid.Parse).ToList();
            query = query.Where(x => brandIds.Contains(x.BrandId ?? Guid.Empty));
        }

        if (req.MinPrice > 0)
        {
            query = query.Where(x => x.Price  >= req.MinPrice);
        }

        if (req.MaxPrice > 0)
        {
            query = query.Where(x => x.Price <= req.MaxPrice);
        }

        return query;
    }
}