namespace DevSync.PocPro.Accounts.Api.Features.Tenants.Grpc;

public class TenantsServicesImpl : TenantService.TenantServiceBase
{
    private readonly IApplicationDbContext _applicationDbContext;

    public TenantsServicesImpl(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    
    public override async Task<GetAllTenantsResponse> GetAllTenants(GetAllTenantsRequest request, ServerCallContext context)
    {
        var allTenants = await _applicationDbContext
            .Tenants
            .Select(t => new GetTenantDetailsResponse
            {
                ConnectionString = t.ConnectionString,
                UniqueIdentifier = t.UniqueIdentifier,
                SubscriptionType = t.SubscriptionType!.Value.ToString(),
            })
            .AsNoTracking()
            .ToListAsync();
        
        var response = new GetAllTenantsResponse
        {
            Tenants = { allTenants }
        };
        
        return response;
    }
    
    public override async Task<GetTenantDetailsResponse> GetTenantDetails(GetTenantDetailsRequest request, ServerCallContext context)
    {
        var user = await _applicationDbContext
            .ApplicationUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == request.UserId);
        
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }
        
        var tenant = await _applicationDbContext
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

    public override async Task<DoesUserRequiredPermissionResponse> DoesUserHaveRequiredPermission(DoesUserRequiredPermissionRequest request, ServerCallContext context)
    {
        var user = await _applicationDbContext
            .ApplicationUsers
            .Include(a => a.Permissions)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.UserId == request.UserId);
        
        if (user == null) return new DoesUserRequiredPermissionResponse { Response = false };
        
        return user.HasPermission(Enum.Parse<PermissionType>(request.Permission))
            ? new DoesUserRequiredPermissionResponse
            {
                Response = true
            }
            : new DoesUserRequiredPermissionResponse
            {
                Response = false
            };
    }
}