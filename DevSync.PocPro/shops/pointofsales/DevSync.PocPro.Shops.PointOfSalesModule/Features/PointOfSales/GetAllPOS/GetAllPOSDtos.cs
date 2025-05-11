namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetAllPOS;

public record GetAllPOSResponse(IEnumerable<POSResponse> PointOfSales);

public record GetAllPOSRequest(
    [Microsoft.AspNetCore.Mvc.FromQuery] string? Keyword, 
    [Microsoft.AspNetCore.Mvc.FromQuery] string UserId);