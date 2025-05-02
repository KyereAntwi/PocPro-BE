namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.EndSession;

public record EndSessionRequest(Guid PosId, Guid SessionId);