namespace DevSync.PocPro.WorkerService.Jobs;

public class CheckItemsLeftOnShelfAgainstLowStockThresholdWorkerService : BackgroundService
{
    private readonly IProductServices _productServices;
    private readonly IAccountServices _accountServices;
    private readonly IPosServices _posServices;
    private readonly ILogger<CheckItemsLeftOnShelfAgainstLowStockThresholdWorkerService> _logger;

    public CheckItemsLeftOnShelfAgainstLowStockThresholdWorkerService(
        IProductServices productServices,
        IAccountServices accountServices,
        IPosServices posServices,
        ILogger<CheckItemsLeftOnShelfAgainstLowStockThresholdWorkerService> logger)
    {
        _productServices = productServices;
        _accountServices = accountServices;
        _posServices = posServices;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var identifiers = await _accountServices.GetAccountIdentifiersAsync(stoppingToken);

            var enumerable = identifiers as string[] ?? identifiers.ToArray();
            if (enumerable.Length != 0)
            {
                foreach (var identifier in enumerable)
                {
                    await PerformInventoryCheckAsync(identifier);
                }
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
    
    private async Task PerformInventoryCheckAsync(string identifier)
    {
        var posIds = await _posServices.GetAllPosIdsAsync(identifier);
        
        if (posIds.Count == 0)
        {
            return;
        }
        
        var productsLowInStock = await _productServices.GetProductsLowInStockAsync(posIds.ToList(), identifier);
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
        }
        catch (Exception e)
        {
            _logger.LogError("Error sending notification for low stock products. Error = {Error}", e.Message);
        }
    }
}