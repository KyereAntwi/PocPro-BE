using DevSync.PocPro.Shared.Domain.Utils;
using DevSync.PocPro.Shops.Shared.Utils;
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
        
        var messageBroker = new MessageBroker();
        configuration.GetSection("MessageBroker").Bind(messageBroker);
        services.AddSingleton(messageBroker);
        
        services.AddTransient<IProductServices, ProductServices>();
        services.AddTransient<IPosServices, PosServices>();
        services.AddTransient<IAccountServices, AccountServices>();
        
        return services;
    }
}