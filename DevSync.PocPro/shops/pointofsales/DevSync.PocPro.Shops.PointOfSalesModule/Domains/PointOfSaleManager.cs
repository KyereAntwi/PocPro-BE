using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.PointOfSales.Domains;

public class PointOfSaleManager : BaseEntity<PosManagerId>
{
    private PointOfSaleManager() { }

    internal PointOfSaleManager(PointOfSaleId pointOfSaleId, string userId)
    {
        Id = PosManagerId.Of(Guid.CreateVersion7());
        PointOfSaleId = pointOfSaleId;
        UserId = userId;
    }

    public PointOfSaleId PointOfSaleId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
}