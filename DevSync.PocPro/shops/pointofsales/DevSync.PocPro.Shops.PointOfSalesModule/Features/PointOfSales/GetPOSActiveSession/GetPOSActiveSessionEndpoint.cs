namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetPOSActiveSession;

public class GetPOSActiveSessionEndpoint(
    IPOSDbContext posDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices)
    : Endpoint<GetPOSActiveSessionRequest, BaseResponse<GetPOSActiveSessionResult>>
{
    public override void Configure()
    {
        Get("/api/v1/pointofsales/{Id}/sessions/active");
    }

    public override async Task HandleAsync(GetPOSActiveSessionRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_SESSIONS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var activeSession = await posDbContext
            .Sessions
            .Where(s => s.PointOfSaleId == PointOfSaleId.Of(req.Id) && s.ClosedAt == null)
            .FirstOrDefaultAsync(ct);

        if (activeSession is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(new BaseResponse<GetPOSActiveSessionResult>("Session fetched", true)
        {
            Data = new GetPOSActiveSessionResult(activeSession.Id.Value, activeSession.CreatedAt, activeSession.CreatedBy!, activeSession.OpeningCash)
        }, ct);
    }
}