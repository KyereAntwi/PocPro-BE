namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalCustomers;

public class TotalCustomersEndpoint(
    IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices, ICustomersExtensionServices customersExtensionServices) 
    : Endpoint<TotalCustomersRequest, BaseResponse<TotalCustomersResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/reports/total-customers");
    }

    public override async Task HandleAsync(TotalCustomersRequest req, CancellationToken ct)
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
        
        var response = await customersExtensionServices.GetTotalCustomersAsync(
            startDate, 
            endDate, 
            ct);
        
        await SendOkAsync(new BaseResponse<TotalCustomersResponse>("Total customers fetched successfully", true)
        {
            Data = new TotalCustomersResponse(
                response.Item1, 
                response.Item2)
        }, ct);
    }
}