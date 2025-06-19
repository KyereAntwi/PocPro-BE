namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetPOS;

public class GetPOSEndpoint(
    IPOSDbContext pocContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<GetPOSRequest, BaseResponse<POSResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/pointofsales/{Id}");
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
                    x.Email ?? string.Empty,
                    x.Address ?? string.Empty,
                    x.Phone ?? string.Empty,
                    x.CreatedAt,
                    x.CreatedBy,
                    x.UpdatedAt,
                    x.UpdatedBy)
            })
            .FirstOrDefaultAsync(x => x.POS.Id == req.Id, ct);

        if (pos is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var response = new BaseResponse<POSResponse>("POS retrieved successfully", true)
        {
            Data = pos.POS
        };
        
        await SendOkAsync(response, cancellation: ct);
    }
}