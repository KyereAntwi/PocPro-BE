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

    public async Task<IEnumerable<PointOfSaleId>> GetAllPosIdsAsync(CancellationToken cancellationToken = default)
    {
        return await posDbContext.PointOfSales
            .Select(p => p.Id)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> PosIsOnlineEnabledAsync(PointOfSaleId posId, CancellationToken cancellationToken = default)
    {
        return await posDbContext.PointOfSales.AnyAsync(p => p.Id == posId && p.OnlineEnabled, cancellationToken);
    }
}