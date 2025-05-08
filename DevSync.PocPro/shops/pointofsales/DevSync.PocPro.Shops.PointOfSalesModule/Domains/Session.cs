using DevSync.PocPro.Shared.Domain.ValueObjects;

namespace DevSync.PocPro.Shops.PointOfSales.Domains;

public class Session : BaseEntity<SessionId>
{
    private Session() { }

    internal Session(PointOfSaleId pointOfSaleId)
    {
        Id = SessionId.Of(Guid.CreateVersion7());
        PointOfSaleId = pointOfSaleId;
    }
    
    public PointOfSaleId PointOfSaleId { get; private set; } = PointOfSaleId.Of(Guid.Empty);
    public string? ClosedBy { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
}