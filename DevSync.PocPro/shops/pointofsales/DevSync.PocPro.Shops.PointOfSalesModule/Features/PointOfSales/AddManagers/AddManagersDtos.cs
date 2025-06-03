using Microsoft.AspNetCore.Mvc;

namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.AddManagers;

public record AddManagersRequest([FromRoute] Guid Id, IEnumerable<string> ManagerUserIds);