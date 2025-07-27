namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalSales;

public record TotalSalesRequest(
    string Pos = "",
    string StartDate = "",
    string EndDate = "");
    
public record TotalSalesResponse(
    decimal TotalSales,
    decimal PercentageChangeFromPreviousPeriod);