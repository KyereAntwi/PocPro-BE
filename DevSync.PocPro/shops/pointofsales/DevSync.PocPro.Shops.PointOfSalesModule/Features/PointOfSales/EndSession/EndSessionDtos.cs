namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.EndSession;

public record EndSessionRequest(Guid Id, Guid SessionId, double ClosingCash);