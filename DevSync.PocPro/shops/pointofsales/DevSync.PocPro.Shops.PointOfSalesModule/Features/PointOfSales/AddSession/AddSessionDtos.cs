namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.AddSession;

public record AddSessionRequest(Guid Id, double OpeningCash);

public record AddSessionResponse(Guid SessionId);