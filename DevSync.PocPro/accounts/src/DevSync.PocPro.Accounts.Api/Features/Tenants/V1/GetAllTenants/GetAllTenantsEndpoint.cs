namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetAllTenants;

public class GetAllTenantsEndpoint(IQueryHandler<GetAllTenantsQuery, List<GetTenantDetailsResponse>> handler) 
    : EndpointWithoutRequest<BaseResponse<IEnumerable<GetTenantDetailsResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/tenants");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var allTenants = await handler.HandleAsync(null!, ct);
        await SendOkAsync(new BaseResponse<IEnumerable<GetTenantDetailsResponse>>("Success", true)
        {
            Data = allTenants.Value
        }, ct);
    }
}