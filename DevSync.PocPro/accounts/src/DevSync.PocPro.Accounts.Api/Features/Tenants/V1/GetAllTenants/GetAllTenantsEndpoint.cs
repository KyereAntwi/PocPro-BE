using DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetTenantDetails;

namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetAllTenants;

public class GetAllTenantsEndpoint(IApplicationDbContext applicationDbContext) 
    : EndpointWithoutRequest<BaseResponse<IEnumerable<GetTenantDetailsResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/tenants");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var allTenants = await applicationDbContext
            .Tenants
            .Select(t => new GetTenantDetailsResponse(
                t.ConnectionString,
                t.Id.Value,
                t.UniqueIdentifier,
                t.SubscriptionType.ToString()))
            .AsNoTracking()
            .ToListAsync(ct);

        await SendOkAsync(new BaseResponse<IEnumerable<GetTenantDetailsResponse>>("Success", true)
        {
            Data = allTenants
        }, ct);
    }
}