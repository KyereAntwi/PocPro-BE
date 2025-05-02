namespace DevSync.PocPro.Shops.StocksModule.Features.Products.StockProduct;

public class StockProductEndpoint(IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<StockProductRequest, BaseResponse<StockProductResponse>>
{
    public override void Configure()
    {
        Put("/products/{ProductId}/stock");
    }

    public override async Task HandleAsync(StockProductRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_PRODUCTS, userId!))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var product = await shopDbContext.Products
            .Include(p => p.Stocks)
            .FirstOrDefaultAsync(p => p.Id == ProductId.Of(req.ProductId), ct);

        if (product == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var result = product.StockProduct(
            req.SupplierId,
            req.QuantityPurchased,
            req.QuantityPurchased,
            req.CostPerPrice,
            req.SellingPerPrice,
            req.TaxRate,
            req.ExpiryAt);
        
        shopDbContext.Products.Update(product);
        await shopDbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}