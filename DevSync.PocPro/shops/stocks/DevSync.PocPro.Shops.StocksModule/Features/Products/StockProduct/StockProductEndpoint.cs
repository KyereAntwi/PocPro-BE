using DevSync.PocPro.Shops.StocksModule.Features.Products.GetStockDetails;

namespace DevSync.PocPro.Shops.StocksModule.Features.Products.StockProduct;

public class StockProductEndpoint(IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<StockProductRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/products/{ProductId}/stocks");
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
        
        var supplier = await shopDbContext.Suppliers.FindAsync(SupplierId.Of(req.SupplierId), ct);

        if (supplier == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var result = product.StockProduct(
            supplier,
            PointOfSaleId.Of(req.ProductId), 
            req.QuantityPurchased,
            req.QuantityPurchased,
            req.CostPerPrice,
            req.SellingPerPrice,
            req.TaxRate,
            req.ExpiryAt);
        
        shopDbContext.Products.Update(product);
        await shopDbContext.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetStockDetailsEndpoint>(new
        {
            ProductId = product.Id.Value,
            StockId = result.Value
        }, new BaseResponse<Guid>("Stock added successfully", true)
        {
            Data = result.Value
        }, cancellation: ct);
    }
}