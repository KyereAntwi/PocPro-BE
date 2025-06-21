namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetManagers;

public class GetManagersEndpoint(
    IPOSDbContext pocContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<GetManagersRequest, BaseResponse<IEnumerable<ManagerResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/pointofsales/{Id}/managers");
    }

    public override async Task HandleAsync(GetManagersRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.GET_POS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var managers = await pocContext
            .PointOfSaleManagers
            .Where(m => m.PointOfSaleId == PointOfSaleId.Of(req.Id))
            .Select(m => new ManagerResponse(m.Id.Value, m.UserId))
            .AsNoTracking()
            .ToListAsync(ct);

        await SendOkAsync(new BaseResponse<IEnumerable<ManagerResponse>>("Managers fetched successfully", true)
        {
            Data = managers
        }, ct);
    }
}