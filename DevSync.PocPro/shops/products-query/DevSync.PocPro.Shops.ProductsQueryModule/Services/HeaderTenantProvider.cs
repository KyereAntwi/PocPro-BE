namespace DevSync.PocPro.Shops.ProductsQueryModule.Services;

public class HeaderTenantProvider(IHttpContextAccessor httpContextAccessor) 
    : ITenantProvider
{
    public Result<string> GetTenantId()
    {
        var context = httpContextAccessor.HttpContext;
        if (context?.Request.Headers.TryGetValue("X-Tenant-Identifier", out var tenantId) == true)
        {
            return tenantId.ToString();
        }

        return Result.Fail("Tenant Identifier not provided");
    }
}