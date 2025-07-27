namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.SalesOverview;

public record SalesOverviewRequest(
    string Pos = "",
    string StartDate = "",
    string EndDate = "");

public record OverviewSummary
{
    public decimal TotalForOnline { get; set; }
    public decimal TotalForPos { get; set; }
}