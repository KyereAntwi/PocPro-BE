namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.SalesOverview;

public class SalesOverviewEndpoint(
    IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices, IOrdersServices ordersServices) 
    : Endpoint<SalesOverviewRequest, BaseResponse<Dictionary<int, OverviewSummary>>>
{
    public override void Configure()
    {
        Get("/api/v1/reports/sales-overview");
    }

    public override async Task HandleAsync(SalesOverviewRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_REPORTS, userId!))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var startDate = DateTimeOffset.TryParse(req.StartDate, out var parsedStartDate) 
            ? parsedStartDate 
            : DateTime.UtcNow.AddMonths(-1);

        var endDate = DateTimeOffset.TryParse(req.EndDate, out var parsedEndDate) 
            ? parsedEndDate 
            : DateTime.UtcNow;
        
        var query = await ordersServices.GetMonthlyGroupedSalesAsync(
            (string.IsNullOrWhiteSpace(req.Pos) ? null : PointOfSaleId.Of(Guid.Parse(req.Pos))),
            startDate, 
            endDate, 
            ct);
        
        var overviewSummary = new Dictionary<int, OverviewSummary>();

        if (query.Count > 0)
        {
            foreach (var (month, (totalForPos, totalForOnline)) in query)
            {
                overviewSummary[month] = new OverviewSummary
                {
                    TotalForPos = totalForPos,
                    TotalForOnline = totalForOnline
                };
            }
        }
        else
        {
            foreach (var month in Enumerable.Range(1, 12))
            {
                overviewSummary[month] = new OverviewSummary
                {
                    TotalForOnline = 0,
                    TotalForPos = 0
                };
            }
        }
        
        await SendOkAsync(new BaseResponse<Dictionary<int, OverviewSummary>>("Sales overview fetched successfully", true)
        {
            Data = overviewSummary
        }, ct);
    }
}