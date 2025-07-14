using DevSync.PocPro.WorkerService.Interfaces;
using IPosServices = DevSync.PocPro.WorkerService.Interfaces.IPosServices;
using IProductServices = DevSync.PocPro.WorkerService.Interfaces.IProductServices;

namespace DevSync.PocPro.WorkerService.Jobs;

public class InventoryNotificationService(
    ILogger<InventoryNotificationService> logger,
    IPublishEndpoint publishEndpoint,
    IPosServices posServices,
    IProductServices productServices,
    IAccountServices accountServices)
{
    public async Task CheckItemsLeftOnShelfAgainstLowStockThresholdAsync()
    {
        logger.LogInformation("Checked items left on shelf against low stock threshold...");
        
        var identifiers = await accountServices.GetAccountIdentifiersAsync();

        if (!identifiers.Any())
        {
            return;
        }

        foreach (var identifier in identifiers)
        {
            await PerformInventoryCheckAsync(identifier);
        }
    }

    public async Task CheckForExpiringProductsAsync()
    {
        var identifiers = await accountServices.GetAccountIdentifiersAsync();
        if (!identifiers.Any())
        {
            return;
        }

        foreach (var identifier in identifiers)
        {
            await PerformExpiringProductsCheckAsync(identifier);
        }
        
        logger.LogInformation("Checked for expiring products...");
    }

    private async Task PerformInventoryCheckAsync(string identifier)
    {
        var posIds = await posServices.GetAllPosIdsAsync(identifier);
        
        if (posIds.Count == 0)
        {
            return;
        }
        
        var productsLowInStock = await productServices.GetProductsLowInStockAsync(posIds.ToList(), identifier);
        var lowInStock = productsLowInStock.ToList();
            
        if (lowInStock.Count == 0)
        {
            return;
        }
        
        try
        {
            var groupedList = lowInStock
                .GroupBy(p => p.PosId)
                .ToDictionary(dtos => dtos.Key, dtos => dtos.ToList());

            var message = groupedList
                .Select(product => 
                    new ProductsForPosDto
                    {
                        PointOfSaleId = PointOfSaleId.Of((Guid)product.Key!), 
                        Products = product.Value
                    })
                .ToList();

            await publishEndpoint.Publish(new SendProductsWithLowStockNotificationEvent
            {
                ProductsForPos = message
            });
        }
        catch (Exception e)
        {
            logger.LogError("Error sending notification for low stock products. Error = {Error}", e.Message);
        }
    }

    private async Task PerformExpiringProductsCheckAsync(string identifier)
    {
        var posIds = await posServices.GetAllPosIdsAsync(identifier);
        
        if (posIds.Count == 0)
        {
            return;
        }
        
        var productsExpiringSoon = await productServices.GetProductsExpiringSoonAsync(posIds.ToList(), identifier);
        var expiringProducts = productsExpiringSoon.ToList();
            
        if (expiringProducts.Count == 0)
        {
            return;
        }
        
        try
        {
            var groupedList = expiringProducts
                .GroupBy(p => p.PosId)
                .ToDictionary(dtos => dtos.Key, dtos => dtos.ToList());
                    
            var message = groupedList
                .Select(product => 
                    new ProductsForPosDto
                    {
                        PointOfSaleId = PointOfSaleId.Of((Guid)product.Key!), 
                        Products = product.Value
                    })
                .ToList();

            await publishEndpoint.Publish(new SendProductsExpiringNotificationEvent
            {
                ProductsForPos = message
            });
        }
        catch (Exception e)
        {
            logger.LogError("Error sending notification for expiring products. Error = {Error}", e.Message);
        }
    }
}