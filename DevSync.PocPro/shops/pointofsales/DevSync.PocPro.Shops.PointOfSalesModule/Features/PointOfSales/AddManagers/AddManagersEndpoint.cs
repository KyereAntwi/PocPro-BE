namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.AddManagers;

public class AddManagersEndpoint (
    IPOSDbContext posDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices)
    : Endpoint<AddManagersRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Put("/api/v1/pointofsales/{Id}/managers/add");
    }

    public override async Task HandleAsync(AddManagersRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_POS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendAsync(new BaseResponse<Guid>("Permission Denied", false)
                {
                    Errors = ["You do not have required permission"]
                }, 
                StatusCodes.Status403Forbidden, ct);
            return;
        }
        
        var pos = await posDbContext.PointOfSales
            .Include(x => x.Managers)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == PointOfSaleId.Of(req.Id), ct);

        if (pos is null)
        {
            await SendAsync(
                new BaseResponse<Guid>("Not Found", false)
                {
                    Errors = [$"POS with Id {req.Id} was not found"]
                }, 
                StatusCodes.Status404NotFound, ct);
            return;
        }

        if (req.ManagerUserIds.Any())
        {
            foreach (var user in req.ManagerUserIds)
            {
                var result = pos.AddManager(user);
                if (result.IsFailed)
                {
                    await SendErrorsAsync((int)HttpStatusCode.BadRequest, ct);
                }
            }
        }
        
        await posDbContext.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}