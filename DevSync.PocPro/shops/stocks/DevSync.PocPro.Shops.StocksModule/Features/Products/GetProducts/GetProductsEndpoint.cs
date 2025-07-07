namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProducts;

public class GetProductsEndpoint(IShopDbContext shopDbContext) 
    : Endpoint<GetProductsRequests, BaseResponse<PagedResponse<GetProductsResponseItem>>>
{
    public override void Configure()
    {
        Get("/api/v1/products");
    }

    public override async Task HandleAsync(GetProductsRequests req, CancellationToken ct)
    {
        var query = shopDbContext
            .Products
            .Include(p => p.Stocks)
            .AsSplitQuery()
            .AsNoTracking();

        query = FilterProductQuery(req, query);

        var totalCount = await query.LongCountAsync(ct);
        
        query = query
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize);
        
        var response = new PagedResponse<GetProductsResponseItem>(
            req.Page,
            req.PageSize,
            totalCount,
            await query.Select(x => new GetProductsResponseItem(
                x.Id.Value,
                x.Name,
                x.CurrentSellingPrice(string.IsNullOrWhiteSpace(req.Pos) ? null : PointOfSaleId.Of(Guid.Parse(req.Pos))),
                x.PhotoUrl,
                x.CategoryId.Value,
                x.Description ?? string.Empty,
                x.LowThresholdValue,
                x.BrandId != null ? x.BrandId.Value : null,
                null
            )).ToArrayAsync(ct)
        );

        await SendAsync(
            new BaseResponse<PagedResponse<GetProductsResponseItem>>("Products fetched successfully", true)
            {
                Data = response
            }, cancellation: ct);
    }

    private static IQueryable<Product> FilterProductQuery(GetProductsRequests req, IQueryable<Product> query)
    {
        if (!string.IsNullOrWhiteSpace(req.Pos))
        {
            query = query.Where(x => x.Stocks.Any(s => s.PointOfSaleId == PointOfSaleId.Of(Guid.Parse(req.Pos))));
        }
        
        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            query = query.Where(x => x.Name.Contains(req.SearchText));
        }
        
        if (!string.IsNullOrWhiteSpace(req.Category))
        {
            query = query.Where(x => x.CategoryId == CategoryId.Of(Guid.Parse(req.Category)));
        }

        if (string.IsNullOrWhiteSpace(req.Brands)) return query;
        {
            var brandIds = req.Brands.Split(';').ToList();
            query = query.Where(x => x.BrandId != null && brandIds.Contains(x.BrandId.Value.ToString()));
        }

        return query;
    }
}