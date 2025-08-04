using System.Security.Claims;
using DevSync.PocPro.Shops.Shared.Interfaces;

namespace DevSync.PocPro.Shops.ProductsQueryModule.Services;

public class HeaderTenantProvider(
    IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : ITenantProvider
{
    public Result<string> GetTenantId()
    {
        var context = httpContextAccessor.HttpContext;
        if (context?.Request.Headers.TryGetValue("X-Tenant-Identifier", out var tenantId) == true)
        {
            return tenantId.ToString();
        }

        if (string.IsNullOrWhiteSpace(context!.User.FindFirstValue(ClaimTypes.NameIdentifier)))
            return Result.Fail("Tenant Identifier not provided");
        
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tenant = tenantServices.GetTenantByUserIdAsync(userId!).GetAwaiter().GetResult();

        if (tenant is null)
        {
            Result.Fail("Tenant Identifier not provided");
        }

        return tenant!.UniqueIdentifier;
    }
}