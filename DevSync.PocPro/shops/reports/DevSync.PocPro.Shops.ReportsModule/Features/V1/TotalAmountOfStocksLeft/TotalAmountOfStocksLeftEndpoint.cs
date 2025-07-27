using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.ReportsModule.Features.V1.TotalAmountOfStocksLeft;

public class TotalAmountOfStocksLeftEndpoint(
    IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices, IProductServices productServices) 
    : Endpoint<TotalAmountOfStocksLeftRequest, BaseResponse<TotalAmountOfStocksLeftResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/reports/total-amount-of-stocks-left");
    }

    public override async Task HandleAsync(TotalAmountOfStocksLeftRequest req, CancellationToken ct)
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
        
        var (totalAmount, percentageChange) = await productServices
            .GetTotalAmountOfStocksLeftAsync(
                string.IsNullOrWhiteSpace(req.Pos) ? null : PointOfSaleId.Of(Guid.Parse(req.Pos)),
                startDate, 
                endDate, 
                ct);

        await SendOkAsync(new BaseResponse<TotalAmountOfStocksLeftResponse>("", true)
        {
            Data = new TotalAmountOfStocksLeftResponse(totalAmount, percentageChange)
        }, ct);
    }
}