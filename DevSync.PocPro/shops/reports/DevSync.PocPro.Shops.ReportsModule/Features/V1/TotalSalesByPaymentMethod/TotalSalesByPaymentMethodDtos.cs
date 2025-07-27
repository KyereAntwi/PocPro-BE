namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalSalesByPaymentMethod;

public record TotalSalesByPaymentMethodRequest(
    string StartDate = "",
    string EndDate = "",
    string Pos = ""
);