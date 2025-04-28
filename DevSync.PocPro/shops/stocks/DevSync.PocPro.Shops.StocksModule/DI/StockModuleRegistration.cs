namespace DevSync.PocPro.Shops.StocksModule.DI;

public static class StockModuleRegistration
{
    public static IServiceCollection AddStockModule(this IServiceCollection services, IConfigurationBuilder configurationBuilder)
    {
        services.AddDbContext<StocksModuleDbContext>(opt =>
        {
            opt.UseNpgsql("postgres");
        });

        services.AddScoped<IShopDbContext, StocksModuleDbContext>();
        
        return services;
    }
}