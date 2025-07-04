using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.RemoveManagers;

public class RemoveManagersEndpoint(
    IPOSDbContext posDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<RemoveManagersRequest>
{
    public override void Configure()
    {
        Put("/api/v1/pointofsales/{Id}/managers/remove");
    }

    public override async Task HandleAsync(RemoveManagersRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_POS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var pos = await posDbContext.PointOfSales
            .Include(x => x.Managers)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == PointOfSaleId.Of(req.Id), ct);

        if (pos is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        if (req.ManageruserIds.Any())
        {
            if (req.ManageruserIds.Select(user => pos.RemoveManager(user)).Any(result => result.IsFailed))
            {
                await SendErrorsAsync((int)HttpStatusCode.BadRequest, ct);
                return;
            }
        }
        
        await posDbContext.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}