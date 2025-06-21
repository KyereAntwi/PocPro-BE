namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetStockDetails;

public class GetStockDetailsEndpoint(
    IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<GetStockDetailsRequest, BaseResponse<StockItem>>
{
    public override void Configure()
    {
        Get("/api/v1/products/{ProductId}/stocks/{StockId}");
    }

    public override async Task HandleAsync(GetStockDetailsRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_STOCKS, userId!))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var stock = await shopDbContext
            .Stocks
            .Select(s => 
                new GetStockDetailsResponse
                {
                    Stock = new StockItem(
                        s.Id.Value,
                        s.QuantityPurchased,
                        s.QuantityLeftInStock,
                        s.CostPerPrice,
                        s.SellingPerPrice,
                        s.TaxRate,
                        s.ExpiresAt)
                    {
                        Supplier = new SupplierItem(s.Supplier.Id.Value, s.Supplier.Title, s.Supplier.Email ?? string.Empty)
                        {
                            Contact = s.Supplier.Contacts.Select(c => new SupplierContactItem(c.Value, c.ContactType.ToString())).ToArray()
                        }
                    }
                })
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Stock.Id == req.StockId, ct);
        
        if (stock == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(new BaseResponse<StockItem>("Stock details retrieved successfully", true)
        {
            Data = stock.Stock
        }, cancellation: ct);
    }
}