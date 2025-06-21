namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetManagers;

public record GetManagersRequest(Guid Id);

public record ManagerResponse(Guid Id, string UserId);