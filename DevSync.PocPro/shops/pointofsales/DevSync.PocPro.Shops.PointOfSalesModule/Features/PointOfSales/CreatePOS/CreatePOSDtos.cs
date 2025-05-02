namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.CreatePOS;

public record CreatePOSRequest(string Title);

public record CreatePOSResponse(Guid Id);