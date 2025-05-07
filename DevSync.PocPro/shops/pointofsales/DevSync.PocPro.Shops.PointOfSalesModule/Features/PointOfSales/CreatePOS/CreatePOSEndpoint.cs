namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.CreatePOS;

public class CreatePOSEndpoint(
    IPOSDbContext posDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<CreatePOSRequest, BaseResponse<CreatePOSResponse>>
{
    public override void Configure()
    {
        Post("/api/v1/pointofsales");
    }

    public override async Task HandleAsync(CreatePOSRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_POS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var newPOS = PointOfSale.Create(req.Title);
        
        await posDbContext.PointOfSales.AddAsync(newPOS, ct);
        await posDbContext.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetPOSEndpoint>(new
            {
                Id = newPOS.Id.Value
            }, new BaseResponse<CreatePOSResponse>("POS created successfully", true)
            {
                Data = new CreatePOSResponse(newPOS.Id.Value)
            },
            cancellation: ct);
    }
}