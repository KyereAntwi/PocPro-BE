namespace DevSync.PocPro.WorkerService.Jobs;

public class CheckForExpiringProductsWorkerService 
    : BackgroundService
{
    private readonly ILogger<CheckForExpiringProductsWorkerService> _logger;
    private readonly IPosServices _posServices;
    private readonly IProductServices _productServices;
    private readonly IAccountServices _accountServices;

    public CheckForExpiringProductsWorkerService(
        ILogger<CheckForExpiringProductsWorkerService> logger,
        IPosServices posServices,
        IProductServices productServices,
        IAccountServices accountServices)
    {
        _logger = logger;
        _posServices = posServices;
        _productServices = productServices;
        _accountServices = accountServices;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var identifiers = await _accountServices.GetAccountIdentifiersAsync();
            var enumerable = identifiers as string[] ?? identifiers.ToArray();
            if (enumerable.Length != 0)
            {
                foreach (var identifier in enumerable)
                {
                    await PerformExpiringProductsCheckAsync(identifier);
                }
            }
            
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
    
    private async Task PerformExpiringProductsCheckAsync(string identifier)
    {
        var posIds = await _posServices.GetAllPosIdsAsync(identifier);
        
        if (posIds.Count == 0)
        {
            return;
        }
        
        var productsExpiringSoon = await _productServices.GetProductsExpiringSoonAsync(posIds.ToList(), identifier);
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
        }
        catch (Exception e)
        {
            _logger.LogError("Error sending notification for expiring products. Error = {Error}", e.Message);
        }
    }
}