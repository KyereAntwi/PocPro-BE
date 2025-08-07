using DevSync.PocPro.Shared.Domain.Utils;
using DevSync.PocPro.Shops.Shared.Events;

namespace DevSync.PocPro.Shops.StocksModule.Services;

public class PurchaseServices(
    StocksModuleDbContext stocksModuleDbContext,
    ILogger<PurchaseServices> logger,
    RabbitMqConnectionFactory rabbitMqConnection,
    ITenantServices tenantServices) 
    : IPurchaseServices
{
    public async Task<Result> MakePurchaseOnProducts(IEnumerable<MakePurchaseOnProductsRequest> requests, CancellationToken cancellationToken)
    {
        var productIds = requests.Select(r => ProductId.Of(r.ProductId)).ToList();
            
        var products = await stocksModuleDbContext
            .Products
            .Include(p => p.Stocks)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        List<PurchaseMadeOnProductDto> itemsToPublish = [];
            
        foreach (var request in requests)
        {
            var product = products.FirstOrDefault(p => p.Id == ProductId.Of(request.ProductId));
            
            if (product is null)
            {
                return Result.Fail($"Product with Id {request.ProductId} was not found");
            }
            
            var purchaseResult = product.MakePurchase(request.Quantity, PointOfSaleId.Of(request.PosId));

            if (!purchaseResult.IsFailed)
            {
                itemsToPublish.Add(new PurchaseMadeOnProductDto(product.Id.Value, request.Quantity));
                continue;
            }

            logger.LogInformation("Attempting purchase on Product {ProductId} failed with error = {Error}", request.ProductId, purchaseResult.Errors[0].Message);
            return Result.Fail(purchaseResult.Errors);
        }
        
        await stocksModuleDbContext.SaveChangesAsync(cancellationToken);

        if (itemsToPublish.Count <= 0) return Result.Ok();
        
        try
        {
            await SendEventForProductPurchased(itemsToPublish, products[0].CreatedBy!, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError("Error publishing product for purchase deduction on product query. Error = {Error}", e.Message);
        }

        return Result.Ok();
    }

    public async Task<Result> ReversePurchaseOnProducts(IEnumerable<MakePurchaseOnProductsRequest> requests, CancellationToken cancellationToken = default)
    {
        var productIds = requests.Select(r => ProductId.Of(r.ProductId)).ToList();
            
        var products = await stocksModuleDbContext
            .Products
            .Include(p => p.Stocks)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
        
        List<ProductCanceledDto> itemsToPublish = [];
        
        foreach (var request in requests)
        {
            var product = products.FirstOrDefault(p => p.Id == ProductId.Of(request.ProductId));
            
            if (product is null)
            {
                return Result.Fail($"Product with Id {request.ProductId} was not found");
            }
            
            var reverseResult = product.ReversePurchase(request.Quantity, PointOfSaleId.Of(request.PosId));

            if (!reverseResult.IsFailed)
            {
                itemsToPublish.Add(new ProductCanceledDto
                {
                    ProductId = product.Id.Value,
                    Quantity = request.Quantity,
                    PosId = request.PosId
                });
                continue;
            }

            logger.LogInformation("Attempting reverse purchase on Product {ProductId} failed with error = {Error}", request.ProductId, reverseResult.Errors[0].Message);
            return Result.Fail(reverseResult.Errors);
        }
        
        await stocksModuleDbContext.SaveChangesAsync(cancellationToken);
        if (itemsToPublish.Count <= 0) return Result.Ok();
        
        try
        {
            await SendOrderCancelledEvent(itemsToPublish, products[0].CreatedBy!, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError("Error publishing product for purchase reversal on product query. Error = {Error}", e.Message);
        }
        
        return Result.Ok();
    }

    private async Task SendEventForProductPurchased(List<PurchaseMadeOnProductDto> items, string createdBy, CancellationToken ct)
    {
        var tenant = await tenantServices.GetTenantByUserIdAsync(createdBy);
        
        var integrationEvent = new PurchaseMadeOnProductEvent
        {
            Items = items,
            TenantIdentifier = tenant!.UniqueIdentifier
        };
            
        var publisher = new EventPublisher(rabbitMqConnection);
        await publisher.PublishAsync(integrationEvent, "shop_exchange", "purchase_changes_in_product_query", ct);
    }
    private async Task SendOrderCancelledEvent(IEnumerable<ProductCanceledDto> items, string userId, CancellationToken ct)
    {
        var tenant = await tenantServices.GetTenantByUserIdAsync(userId);
        
        var integrationEvent = new OrderCanceledEvent
        {
            Items = items,
            TenantIdentifier = tenant!.UniqueIdentifier
        };

        var publisher = new EventPublisher(rabbitMqConnection);
        await publisher.PublishAsync(integrationEvent, "shop_exchange", "purchase_changes_in_product_query", ct);
    }
}