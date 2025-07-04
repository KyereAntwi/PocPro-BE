using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.PointOfSales.Services;

public class PosServices(IPOSDbContext posDbContext) : IPosServices
{
    public async Task<IEnumerable<PointOfSaleId>> GetOnlineEnabledPosIdsAsync(CancellationToken cancellationToken = default)
    {
        return await posDbContext.PointOfSales.Where(p => p.OnlineEnabled)
            .Select(p => p.Id)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }
}