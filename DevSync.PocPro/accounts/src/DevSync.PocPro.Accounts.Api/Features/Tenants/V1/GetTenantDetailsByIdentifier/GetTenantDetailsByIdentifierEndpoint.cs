using DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetTenantDetails;

namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetTenantDetailsByIdentifier;

public class GetTenantDetailsByIdentifierEndpoint(IApplicationDbContext applicationDbContext) 
    : Endpoint<GetTenantDetailsByIdentifierRequest, BaseResponse<GetTenantDetailsResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/tenants/byidentifier/{Identifier}");
        Description(x => x
            .WithName("GetTenantDetailsByIdentifier")
            .Produces<BaseResponse<GetTenantDetailsResponse>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound));
        
        AllowAnonymous();
    }

    public override  async Task HandleAsync(GetTenantDetailsByIdentifierRequest req, CancellationToken ct)
    {
        var tenant = await applicationDbContext.Tenants.FirstOrDefaultAsync(t => t.UniqueIdentifier == req.Identifier, ct);
        
        if (tenant == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(new BaseResponse<GetTenantDetailsResponse>("Success", true)
        {
            Data = new GetTenantDetailsResponse(tenant.ConnectionString, tenant.Id.Value, tenant.UniqueIdentifier, tenant.SubscriptionType.ToString())
        }, cancellation: ct);
    }
}