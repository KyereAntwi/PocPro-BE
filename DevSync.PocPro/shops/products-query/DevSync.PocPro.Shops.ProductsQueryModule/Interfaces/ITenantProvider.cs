namespace DevSync.PocPro.Shops.ProductsQueryModule.Interfaces;

public interface ITenantProvider
{
    Result<string> GetTenantId();
}