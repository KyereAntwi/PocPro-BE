namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetPOSActiveSession;

public record GetPOSActiveSessionRequest(Guid Id);

public record GetPOSActiveSessionResult(Guid Id, DateTimeOffset? CreatedAt,  string CreatedBy, double OpeningCash);