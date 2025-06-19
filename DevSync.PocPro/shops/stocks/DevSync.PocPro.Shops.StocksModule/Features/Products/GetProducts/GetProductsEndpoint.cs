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
        
        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            query = query.Where(x => x.Name.Contains(req.SearchText));
        }
        
        if (req.CategoryId != Guid.Empty)
        {
            query = query.Where(x => x.CategoryId == CategoryId.Of(req.CategoryId));
        }
        
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
                x.CurrentSellingPrice(),
                x.PhotoUrl,
                x.TotalNumberLeftOnShelf(),
                x.CategoryId.Value,
                x.Description ?? string.Empty,
                x.LowThresholdValue
            )).ToArrayAsync(ct)
        );

        await SendAsync(
            new BaseResponse<PagedResponse<GetProductsResponseItem>>("Products fetched successfully", true)
            {
                Data = response
            }, cancellation: ct);
    }
}