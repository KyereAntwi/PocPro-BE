namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetAllPOS;

public class GetAllPOSEndpoint(
    IPOSDbContext pocContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices)
    : EndpointWithoutRequest<BaseResponse<GetAllPOSResponse>>
{
    public override void Configure()
    {
        Get("/pointofsales");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.GET_POS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var pos = await pocContext.PointOfSales
            .Select(x => new POSResponse(
                x.Id.Value,
                x.Title,
                x.CreatedAt,
                x.CreatedBy,
                x.UpdatedAt,
                x.UpdatedBy))
            .ToListAsync(ct);

        var response = new BaseResponse<GetAllPOSResponse>("", true)
        {
            Data = new GetAllPOSResponse(pos)
        };
        
        await SendOkAsync(response, cancellation: ct);
    }
}