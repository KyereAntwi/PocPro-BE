using DevSync.PocPro.Shops.Shard.Grpc;
using Microsoft.Extensions.Logging;

namespace DevSync.PocPro.Shops.Shared.Services;

public class TenantServices(TenantService.TenantServiceClient tenantServiceClient, ILogger<TenantServices> logger) : ITenantServices
{
    public async Task<Tenant?> GetTenantByUserIdAsync(string userId)
    {
        try
        {
            var request = new GetTenantDetailsRequest { UserId = userId };
            var response = await tenantServiceClient.GetTenantDetailsAsync(request);

            return new Tenant(response.ConnectionString, userId, "DefaultSubscription");
        }
        catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            logger.LogError("Tenant not found for user {UserId}: {Message}", userId, ex.Message);
            throw new Exception("Tenant not found", ex);
        }
        catch (Exception e)
        {
            logger.LogError("Error fetching tenant details for user {UserId}: {Message}", userId, e.Message);
            throw new Exception("Error fetching tenant details", e);
        }
    }

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
    {
        try
        {
            var request = new GetAllTenantsRequest();
            var response = await tenantServiceClient.GetAllTenantsAsync(request);

            return response.Tenants.Select(t => new Tenant(t.ConnectionString, t.UniqueIdentifier, t.SubscriptionType));
        }
        catch (Exception e)
        {
            logger.LogError("Error fetching tenants: {Message}", e.Message);
            throw new Exception("Error fetching tenants", e);
        }
    }
}