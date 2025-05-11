namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.RemoveManagers;

public record RemoveManagersRequest(Guid Id, IEnumerable<string> ManageruserIds);