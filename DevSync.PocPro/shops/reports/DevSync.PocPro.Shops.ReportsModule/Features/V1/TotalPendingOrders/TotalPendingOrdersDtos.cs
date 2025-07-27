namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalPendingOrders;

public record TotalPendingOrdersRequest(
    string Pos = "",
    string StartDate = "",
    string EndDate = ""
);

public record TotalPendingOrdersResponse(
    int TotalPendingOrders,
    decimal PercentageChange
);