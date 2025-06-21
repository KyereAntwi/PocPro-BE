namespace DevSync.PocPro.Shops.StocksModule.Services;

public class PurchaseServices(
    StocksModuleDbContext stocksModuleDbContext,
    ILogger<PurchaseServices> logger) 
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
            
        foreach (var request in requests)
        {
            var product = products.FirstOrDefault(p => p.Id == ProductId.Of(request.ProductId));
            
            if (product is null)
            {
                return Result.Fail($"Product with Id {request.ProductId} was not found");
            }
            
            var purchaseResult = product.MakePurchase(request.Quantity, PointOfSaleId.Of(request.PosId));
            
            if (!purchaseResult.IsFailed) continue;
            
            logger.LogInformation("Attempting purchase on Product {ProductId} failed with error = {Error}", request.ProductId, purchaseResult.Errors[0].Message);
            return Result.Fail(purchaseResult.Errors);
        }
        
        await stocksModuleDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}