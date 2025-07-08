namespace DevSync.PocPro.Shops.WorkerService.DI;

public static class RegisterWorkerServiceDependencies
{
    public static IServiceCollection AddWorkerServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => 
            config.UsePostgreSqlStorage(configuration.GetConnectionString("HangfireDbConnection")));
        
        services.AddHangfireServer();

        services.AddScoped<InventoryNotificationService>();
        
        return services;
    }
}