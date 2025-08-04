using DevSync.PocPro.Shared.Domain.Utils;
using DevSync.PocPro.Shops.StocksModule.Features.Products.GetStockDetails;

namespace DevSync.PocPro.Shops.StocksModule.Features.Products.StockProduct;

public class StockProductEndpoint(
    IShopDbContext shopDbContext, 
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices,
    RabbitMqConnectionFactory rabbitMqConnection,
    ILogger<StockProductEndpoint> logger,
    IPosServices posServices,
    IReviewsServices reviewsServices) 
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
        
        var product = await shopDbContext
            .Products
            .Include(p => p.Stocks)
            .Where(p => p.Id == ProductId.Of(req.ProductId))
            .FirstOrDefaultAsync(ct);

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
            PointOfSaleId.Of(req.PosId), 
            req.QuantityPurchased,
            req.QuantityPurchased,
            req.CostPerPrice,
            req.SellingPerPrice,
            req.TaxRate,
            req.ExpiryAt);
        
        shopDbContext.Products.Update(product);

        try
        {
            await shopDbContext.SaveChangesAsync(ct);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        try
        {
            var brandTask = shopDbContext.Brands.FindAsync(product.BrandId, ct);
            var categoryTask = shopDbContext.Categories.FindAsync(product.CategoryId, ct);
            var posIsOnlineEnabledTask = posServices.PosIsOnlineEnabledAsync(PointOfSaleId.Of(req.PosId), ct);
            var tenantTask = tenantServices.GetTenantByUserIdAsync(userId!);
            var ratingsTask = reviewsServices.GetProductRatingAsync(product.Id, ct);
            
            await Task.WhenAll(
                brandTask.AsTask(),
                categoryTask.AsTask(),
                posIsOnlineEnabledTask,
                tenantTask,
                ratingsTask
            );
            
            var brand = await brandTask;
            var category = await categoryTask;
            var posIsOnlineEnabled = await posIsOnlineEnabledTask;
            var tenant = await tenantTask;
            var ratings = await ratingsTask;
            
            var _event = new AddProductToQueryEvent
            {
                TenantIdentifier = tenant!.UniqueIdentifier,
                ProductId = product.Id.Value,
                Name = product.Name,
                PhotoUrl = product.PhotoUrl ?? string.Empty,
                CurrentPrice = req.SellingPerPrice,
                BrandId = brand is null ? Guid.Empty : brand.Id.Value,
                BrandTitle = brand is null ? string.Empty : brand.Title,
                CategoryId = category!.Id.Value,
                CategoryTitle = category.Title,
                QuantityLeft = product.TotalNumberLeftOnShelf(PointOfSaleId.Of(req.PosId)),
                PosId = req.PosId,
                BarcodeNumber = product.BarcodeNumber ?? string.Empty,
                IsOnline = posIsOnlineEnabled,
                Ratings = ratings
            };
            
            var publisher = new EventPublisher(rabbitMqConnection);
            await publisher.PublishAsync(_event, "shop_exchange", "add_products_to_query", ct);
        }
        catch (Exception e)
        {
            logger.LogError("There was a problem publishing product created event. Error = {Error}", e.Message);
        }

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