namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.AddSession;

public class AddSessionEndpoint(
    IPOSDbContext posDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<AddSessionRequest, BaseResponse<AddSessionResponse>>
{
    public override void Configure()
    {
        Put("/api/v1/pointofsales/{Id}/sessions");
    }

    public override async Task HandleAsync(AddSessionRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_POS, userId);
        
        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var pos = await posDbContext.PointOfSales
            .Include(x => x.Sessions)
            .FirstOrDefaultAsync(x => x.Id == PointOfSaleId.Of(req.Id), ct);

        if (pos is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var result = pos.StartSession();

        if (result.IsFailed)
        {
            await SendErrorsAsync((int)StatusCodes.Status400BadRequest, ct);
            return;
        }
        
        await posDbContext.SaveChangesAsync(ct);
        
        await SendOkAsync(ct);
    }
}