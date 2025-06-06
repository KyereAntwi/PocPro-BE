using DevSync.PocPro.Shops.Shared.Dtos;
using DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductDetails;

namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetStocksByProductId;

public class GetStocksByProductIdEndpoint(IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<GetStocksByProductIdRequest, BaseResponse<PagedResponse<StockItem>>>
{
    public override void Configure()
    {
        Get("/api/v1/products/{ProductId}/stocks");
    }

    public override async Task HandleAsync(GetStocksByProductIdRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_STOCKS, userId!))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var query = shopDbContext.Stocks.AsNoTracking();

        var totalCount = await query.LongCountAsync(ct);
        
        query = query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize);
        
        var response = new PagedResponse<StockItem>(
            page: req.Page,
            pageSize: req.PageSize,
            totalCount: totalCount,
            items: await query.Select(s => new StockItem(
                s.Id.Value,
                s.QuantityPurchased,
                s.QuantityLeftInStock,
                s.CostPerPrice,
                s.SellingPerPrice,
                s.TaxRate,
                s.ExpiresAt))
                .ToArrayAsync(ct));
        
        await SendAsync(
            new BaseResponse<PagedResponse<StockItem>>("Stocks fetched successfully", true)
            {
                Data = response
            }, cancellation: ct);
    }
}