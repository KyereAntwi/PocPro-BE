namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface ICustomersExtensionServices
{
    Task<(int, decimal)> GetTotalCustomersAsync(
        DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
}