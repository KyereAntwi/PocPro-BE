namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.CreatePOS;

public record CreatePOSRequest(string Title, string? Email, string? Address, string? Phone);

public record CreatePOSResponse(Guid Id);