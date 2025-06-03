namespace DevSync.PocPro.Shops.PointOfSales.DI;

public static class RegisterPosDependencies
{
    public static IServiceCollection AddPosDependencies(this IServiceCollection services)
    {
        services.AddDbContext<POSModuleDbContext>();
        
        services.AddScoped<IPOSDbContext, POSModuleDbContext>();
        return services;
    }
}