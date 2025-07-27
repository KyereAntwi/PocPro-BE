namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalCustomers;

public record TotalCustomersRequest(
    string StartDate = "",
    string EndDate = "");

public record TotalCustomersResponse(
    int TotalCustomers,
    decimal PercentageChangeFromPreviousPeriod);