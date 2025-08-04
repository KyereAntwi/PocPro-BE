namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalSales;

public class TotalSalesEndpoint(
    IOrdersServices ordersServices, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<TotalSalesRequest, BaseResponse<TotalSalesResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/reports/total-sales");
    }

    public override async Task HandleAsync(TotalSalesRequest req, CancellationToken ct)
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
        
        var (totalSales, percentageChange) = await ordersServices.GetTotalSalesAsync(
            string.IsNullOrWhiteSpace(req.Pos) ? null : PointOfSaleId.Of(Guid.Parse(req.Pos)),
            startDate, 
            endDate, 
            null,
            ct);
        
        await SendOkAsync(new BaseResponse<TotalSalesResponse>("Total sales fetched successfully", true)
        {
            Data = new TotalSalesResponse(totalSales, percentageChange)
        }, ct);
    }
}