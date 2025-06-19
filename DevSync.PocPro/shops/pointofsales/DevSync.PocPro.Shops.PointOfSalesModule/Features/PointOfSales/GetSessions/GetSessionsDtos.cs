using Microsoft.AspNetCore.Mvc;

namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetSessions;

public record GetSessionsRequest(
    [FromRoute] Guid Id,
    int PageSize = 20,
    int Page = 1,
    string DateFrom = "",
    string DateTo = "");

public record SessionResponse(Guid Id, double OpeningCash, double ClosingCash, DateTimeOffset? CreatedAt, DateTimeOffset? ClosedAt, string? ClosedBy, string? CreatedBy);