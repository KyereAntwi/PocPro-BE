namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetAllPOS;

public record GetAllPOSResponse(IEnumerable<POSResponse> PointOfSales);

public record GetAllPOSRequest(
    string Keyword = "", 
    string UserId = "");