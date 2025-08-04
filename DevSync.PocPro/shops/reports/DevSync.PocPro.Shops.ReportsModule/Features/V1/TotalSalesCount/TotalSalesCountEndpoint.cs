namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalSalesCount;

public class TotalSalesCountEndpoint(
    IOrdersServices ordersServices, 
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices) 
    : Endpoint<TotalSalesCountRequest, BaseResponse<int>>
{
    public override void Configure()
    {
        Get("/api/v1/reports/total-sales-count");
    }

    public override async Task HandleAsync(TotalSalesCountRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_REPORTS, userId!))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var startDate = DateTimeOffset.TryParse(req.StartDate, out var parsedStartDate) 
            ? parsedStartDate 
            : new DateTimeOffset();

        var endDate = DateTimeOffset.TryParse(req.EndDate, out var parsedEndDate) 
            ? parsedEndDate 
            : new DateTimeOffset();
        
        var totalCount = await ordersServices.GetTotalSalesCountAsync(
            string.IsNullOrEmpty(req.Pos) ? null : PointOfSaleId.Of(Guid.Parse(req.Pos)),
            startDate, 
            endDate, 
            ct);

        await SendOkAsync(new BaseResponse<int>("Total sales count retrieved successfully.", true)
        {
            Data = totalCount
        }, ct);
    }
}

public record TotalSalesCountRequest(
    string Pos = "",
    string StartDate = "",
    string EndDate = "");