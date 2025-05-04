namespace DevSync.PocPro.Shops.PointOfSales.DI;

public static class RegisterPosDependencies
{
    public static IServiceCollection AddPosDependencies(this IServiceCollection services)
    {
        services.AddDbContext<POSModuleDbContext>(opt =>
        {
            opt.UseNpgsql("postgres");
        });
        
        services.AddScoped<IPOSDbContext, POSModuleDbContext>();
        return services;
    }
}