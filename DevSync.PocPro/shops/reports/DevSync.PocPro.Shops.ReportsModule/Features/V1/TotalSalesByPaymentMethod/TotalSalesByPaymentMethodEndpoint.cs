namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalSalesByPaymentMethod;

public class TotalSalesByPaymentMethodEndpoint(
    IOrdersServices ordersServices, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<TotalSalesByPaymentMethodRequest, BaseResponse<Dictionary<string, decimal>>>
{
    public override void Configure()
    {
        Get("/api/v1/reports/total-sales-by-payment-method");
    }

    public override async Task HandleAsync(TotalSalesByPaymentMethodRequest req, CancellationToken ct)
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

        var response = new Dictionary<string, decimal>();

        var query = await ordersServices.GetGroupedSalesByPaymentMethodAsync(
            string.IsNullOrWhiteSpace(req.Pos) ? null : PointOfSaleId.Of(Guid.Parse(req.Pos)),
            startDate,
            endDate,
            ct);

        if (query.Count > 0)
        {
            response = query;
        }
        else
        {
            response = new Dictionary<string, decimal>
            {
                { "CreditCard", 0m },
                { "Cash", 0m },
                { "MobilePayment", 0m },
                {"BankTransfer", 0m}
            };
        }

        await SendOkAsync(
            new BaseResponse<Dictionary<string, decimal>>("Total sales by payment method fetched successfully", true)
            {
                Data = response
            }, ct);
    }
}