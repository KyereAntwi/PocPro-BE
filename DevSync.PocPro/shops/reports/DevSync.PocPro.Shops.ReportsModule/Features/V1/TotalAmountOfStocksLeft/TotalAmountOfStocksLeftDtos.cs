namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalAmountOfStocksLeft;

public record TotalAmountOfStocksLeftRequest(
    string Pos = "",
    string StartDate = "",
    string EndDate = "");

public record TotalAmountOfStocksLeftResponse(decimal TotalAmountOfStocksLeft, decimal PercentageChange);