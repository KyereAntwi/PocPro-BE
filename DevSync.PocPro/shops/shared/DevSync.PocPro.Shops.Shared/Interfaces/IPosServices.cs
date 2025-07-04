using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IPosServices
{
    Task<IEnumerable<PointOfSaleId>> GetOnlineEnabledPosIdsAsync(CancellationToken cancellationToken = default);
}