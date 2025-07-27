namespace DevSync.PocPro.Shops.GeneralSettings.DI;

public static class RegisterGeneralSettingsModule
{
    public static IServiceCollection AddGeneralSettingsModule(this IServiceCollection services)
    {
        services.AddDbContext<GeneralSettingsModuleDbContext>();
        services.AddScoped<IGeneralSettingsModuleDbContext, GeneralSettingsModuleDbContext>();
        
        return services;
    }
}