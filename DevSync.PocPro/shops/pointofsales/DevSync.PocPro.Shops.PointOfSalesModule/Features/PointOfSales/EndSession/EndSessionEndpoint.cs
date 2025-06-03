namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.EndSession;

public class EndSessionEndpoint(
    IPOSDbContext posDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<EndSessionRequest>
{
    public override void Configure()
    {
        Put("/api/v1/pointofsales/{Id}/sessions/{SessionId}");
    }

    public override async Task HandleAsync(EndSessionRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor?.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_POS, userId!);

        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var pos = await posDbContext.PointOfSales.FirstOrDefaultAsync(p => p.Id == PointOfSaleId.Of(req.Id), ct);

        if (pos is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var result = pos.EndSession(req.SessionId, userId!, req.ClosingCash);

        if (result.IsFailed)
        {
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
        }
        
        await posDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}