namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalPendingOrders;

public class TotalPendingOrdersEndpoint(
    IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices, IOrdersServices ordersServices) 
    : Endpoint<TotalPendingOrdersRequest, BaseResponse<TotalPendingOrdersResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/reports/total-pending-orders");
    }

    public override async Task HandleAsync(TotalPendingOrdersRequest req, CancellationToken ct)
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
        
        var (totalOrders, percentageChange) = await ordersServices.GetTotalPendingOrdersAsync(
            string.IsNullOrWhiteSpace(req.Pos) ? null : PointOfSaleId.Of(Guid.Parse(req.Pos)),
            startDate, 
            endDate, 
            ct);

        await SendOkAsync(new BaseResponse<TotalPendingOrdersResponse>("", true)
        {
            Data = new TotalPendingOrdersResponse(totalOrders, percentageChange)
        }, ct);
    }
}