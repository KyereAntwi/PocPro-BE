namespace DevSync.PocPro.Shops.PromoCodesModule.DI;

public static class RegisterPromoCodesModuleDependencies
{
    public static IServiceCollection AddPromoCodesModuleDependencies(this IServiceCollection services)
    {
        services.AddDbContext<PromoCodeModuleDbContext>();
        
        services.AddScoped<IPromoCodeModuleDbContext, PromoCodeModuleDbContext>();
        services.AddScoped<IPromoCodesServices, PromoCodesServices>();
        
        return services;
    }
}