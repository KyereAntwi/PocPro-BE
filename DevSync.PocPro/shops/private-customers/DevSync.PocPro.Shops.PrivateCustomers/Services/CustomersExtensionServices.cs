namespace DevSync.PocPro.Shops.PrivateCustomers.Services;

public class CustomersExtensionServices(CustomerModuleDbContext dbContext) 
    : ICustomersExtensionServices
{
    public async Task<(int, decimal)> GetTotalCustomersAsync(DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        var totalCustomers = await dbContext.Customers
            .CountAsync(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate, cancellationToken);
        
        var totalForPreviousPeriod =  await dbContext.Customers
            .Where(c => c.CreatedAt >= startDate.AddMonths(-1) && c.CreatedAt < startDate)
            .CountAsync(cancellationToken);
        
        var percentageChange = totalForPreviousPeriod == 0
            ? 0
            : ((totalCustomers - totalForPreviousPeriod) / (decimal)totalForPreviousPeriod) * 100;
        
        return (totalCustomers, percentageChange);
    }
}