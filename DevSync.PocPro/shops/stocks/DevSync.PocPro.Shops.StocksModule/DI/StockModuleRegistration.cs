using DevSync.PocPro.Shops.StocksModule.Services;

namespace DevSync.PocPro.Shops.StocksModule.DI;

public static class StockModuleRegistration
{
    public static IServiceCollection AddStockModule(this IServiceCollection services, IConfigurationBuilder configurationBuilder)
    {
        services.AddDbContext<StocksModuleDbContext>();

        services.AddScoped<IShopDbContext, StocksModuleDbContext>();
        services.AddScoped<IProductServices, ProductServices>();
        
        return services;
    }
}