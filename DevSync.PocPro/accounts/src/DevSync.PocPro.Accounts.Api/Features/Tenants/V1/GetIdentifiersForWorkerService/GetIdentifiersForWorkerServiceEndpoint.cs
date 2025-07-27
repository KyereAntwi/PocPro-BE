namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetIdentifiersForWorkerService;

public class GetIdentifiersForWorkerServiceEndpoint(
    IApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor, ApiAuthentication authentication) 
    : EndpointWithoutRequest<IEnumerable<string>>
{
    public override void Configure()
    {
        Get("/api/v1/tenants/identifiers/for-worker-service");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var apiToken = httpContextAccessor.HttpContext?.Request.Headers["X-Api-Token"].ToString();
        
        if (string.IsNullOrEmpty(apiToken))
        {
            await SendUnauthorizedAsync(cancellation: ct);
            return;
        }

        if (!apiToken.Equals(authentication.ApiKey))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var tenantIdentifiers = await applicationDbContext
            .Tenants
            .Select(t => t.UniqueIdentifier)
            .ToListAsync(ct);
        
        await SendOkAsync(tenantIdentifiers, ct);
    }
}