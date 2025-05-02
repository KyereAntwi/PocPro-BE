namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetPOS;

public class GetPOSEndpoint(
    IPOSDbContext pocContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<GetPOSRequest, BaseResponse<GetPOSResponse>>
{
    public override void Configure()
    {
        Get("/pointofsales/{Id}");
    }

    public override async Task HandleAsync(GetPOSRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.GET_POS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var pos = await pocContext.PointOfSales
            .Select(x => new GetPOSResponse
            {
                POS = new POSResponse(
                    x.Id.Value,
                    x.Title,
                    x.CreatedAt,
                    x.CreatedBy,
                    x.UpdatedAt,
                    x.UpdatedBy),
                
                Sessions = x.Sessions.Select(s => new GetSessionResponse(
                    s.Id.Value, 
                    s.CreatedAt, 
                    s.ClosedAt, 
                    s.ClosedBy, 
                    s.CreatedBy))
            })
            .FirstOrDefaultAsync(x => x.POS.Id == req.Id, ct);

        if (pos is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var response = new BaseResponse<GetPOSResponse>("POS retrieved successfully", true)
        {
            Data = pos
        };
    }
}