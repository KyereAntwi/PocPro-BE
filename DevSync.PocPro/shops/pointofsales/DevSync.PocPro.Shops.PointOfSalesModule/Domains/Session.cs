using DevSync.PocPro.Shared.Domain.ValueObjects;

namespace DevSync.PocPro.Shops.PointOfSales.Domains;

public class Session : BaseEntity<SessionId>
{
    private Session() { }

    internal Session(PointOfSaleId pointOfSaleId, double openingCash)
    {
        Id = SessionId.Of(Guid.CreateVersion7());
        PointOfSaleId = pointOfSaleId;
        OpeningCash = openingCash;
    }

    internal void EndSession(string userId, double closingCash)
    {
        ClosingCash = closingCash;
        ClosedBy = userId;
        ClosedAt = DateTimeOffset.UtcNow;
    }
    
    public PointOfSaleId PointOfSaleId { get; private set; } = PointOfSaleId.Of(Guid.Empty);
    public string? ClosedBy { get; set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public double OpeningCash { get; private set; }
    public double ClosingCash { get; private set; }
}