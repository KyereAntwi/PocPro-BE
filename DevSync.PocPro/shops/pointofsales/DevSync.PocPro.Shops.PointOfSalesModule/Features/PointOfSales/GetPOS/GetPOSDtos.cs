namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetPOS;

public record GetPOSRequest(Guid Id);

public record GetPOSResponse
{
    public POSResponse POS { get; set; }
}

public record POSResponse(
    Guid Id,
    string Title,
    string Email,
    string Address,
    string Phone,
    bool OnlineEnabled,
    DateTimeOffset? CreatedAt,
    string? CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy);