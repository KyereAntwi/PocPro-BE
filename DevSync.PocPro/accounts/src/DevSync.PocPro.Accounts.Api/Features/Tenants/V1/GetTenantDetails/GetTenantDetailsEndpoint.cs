namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetTenantDetails;

public class GetTenantDetailsEndpoint(IApplicationDbContext applicationDbContext) 
    : Endpoint<GetTenantDetailsRequest, BaseResponse<GetTenantDetailsResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/tenants/{UserId}");
        Description(x => x
            .WithName("GetTenantDetails")
            .Produces<BaseResponse<GetTenantDetailsResponse>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(GetTenantDetailsRequest req, CancellationToken ct)
    {
        var user = await applicationDbContext
            .ApplicationUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == req.UserId, ct);
        
        if (user == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var tenant = await applicationDbContext
            .Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == user.TenantId, ct);
        
        if (tenant == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(new BaseResponse<GetTenantDetailsResponse>("Success", true)
        {
            Data = 
                new GetTenantDetailsResponse(tenant.ConnectionString, tenant.Id.Value, tenant.UniqueIdentifier, tenant.SubscriptionType.ToString())
        }, cancellation: ct);
    }
}