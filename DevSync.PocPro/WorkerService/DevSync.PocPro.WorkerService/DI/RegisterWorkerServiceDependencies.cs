using DevSync.PocPro.Shops.Shared.Utils;
using DevSync.PocPro.WorkerService.Interfaces;
using DevSync.PocPro.WorkerService.Jobs;
using DevSync.PocPro.WorkerService.Services;
using DevSync.PocPro.WorkerService.Utils;

namespace DevSync.PocPro.WorkerService.DI;

public static class RegisterWorkerServiceDependencies
{
    public static IServiceCollection AddWorkerServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var externalUrls = new ExternalUrls();
        configuration.GetSection("ExternalUrls").Bind(externalUrls);
        services.AddSingleton(externalUrls);
        
        var tenantServiceSettings = new TenantServiceSettings();
        configuration.GetSection("TenantServiceSettings").Bind(tenantServiceSettings);
        services.AddSingleton(tenantServiceSettings);
        
        services.AddHangfire(config =>
                config.UsePostgreSqlStorage(
                    configuration.GetConnectionString("HangfireConnection"),
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = "hangfire",
                        PrepareSchemaIfNecessary = true
                    }
                ));
        
        services.AddHangfireServer();
        
        services.AddScoped<InventoryNotificationService>();
        services.AddTransient<IProductServices, ProductServices>();
        services.AddTransient<IPosServices, PosServices>();
        services.AddTransient<IAccountServices, AccountServices>();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                { 
                    host.Username(configuration["MessageBroker:Username"]!); 
                    host.Password(configuration["MessageBroker:Password"]!); 
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }
}