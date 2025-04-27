namespace DevSync.PocPro.Accounts.Api.Features.Tenants.Grpc;

public class TenantServicesImpl(IApplicationDbContext applicationDbContext)
    : TenantService.TenantServiceBase
{
    public override async Task<GetTenantDetailsResponse> GetTenantDetails(GetTenantDetailsRequest request, ServerCallContext context)
    {
        var user = await applicationDbContext
            .ApplicationUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == request.UserId);
        
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }
        
        var tenant = await applicationDbContext
            .Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == user.TenantId);
        
        if (tenant == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Tenant not found"));
        }
        
        var response = new GetTenantDetailsResponse
        {
            ConnectionString = tenant.ConnectionString,
            UniqueIdentifier = tenant.UniqueIdentifier,
            SubscriptionType = tenant.SubscriptionType!.Value.ToString(),
        };
        
        return response;
    }
}