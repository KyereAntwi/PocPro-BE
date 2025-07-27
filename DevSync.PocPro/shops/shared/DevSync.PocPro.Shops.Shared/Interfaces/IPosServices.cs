using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IPosServices
{
    Task<IEnumerable<PointOfSaleId>> GetOnlineEnabledPosIdsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PointOfSaleId>> GetAllPosIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> PosIsOnlineEnabledAsync(PointOfSaleId posId, CancellationToken cancellationToken = default);
}