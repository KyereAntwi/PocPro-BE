using DevSync.PocPro.Shops.Shared.Dtos;

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
        
        if (req.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == CategoryId.Of(req.CategoryId.Value));
        }
        
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(x => new GetProductsResponseItem(
                x.Id.Value,
                x.Name,
                x.Stocks.Last().SellingPerPrice,
                x.PhotoUrl,
                x.CategoryId.Value
            ))
            .ToListAsync(ct);
        
        var response = new PagedResponse<GetProductsResponseItem>(
            req.Page,
            req.PageSize,
            totalCount,
            items
        );
        
        await SendAsync(new BaseResponse<PagedResponse<GetProductsResponseItem>>("", true)
        {
            Data = response
        }, cancellation: ct);
    }
}