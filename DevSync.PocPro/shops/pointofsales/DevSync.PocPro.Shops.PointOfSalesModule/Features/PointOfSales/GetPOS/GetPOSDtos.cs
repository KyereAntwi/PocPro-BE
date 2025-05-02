namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetPOS;

public record GetPOSRequest(Guid Id);

public record GetPOSResponse
{
    public POSResponse POS { get; set; }
    public IEnumerable<GetSessionResponse> Sessions { get; set; } = [];
}

public record POSResponse(
    Guid Id,
    string Title,
    DateTimeOffset? CreatedAt,
    string? CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy);

public record GetSessionResponse(Guid Id, DateTimeOffset? CreatedAt, DateTimeOffset? ClosedAt, string? ClosedBy, string? CreatedBy);