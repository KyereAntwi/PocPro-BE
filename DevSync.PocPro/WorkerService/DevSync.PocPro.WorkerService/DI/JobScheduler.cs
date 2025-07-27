using DevSync.PocPro.WorkerService.Jobs;

namespace DevSync.PocPro.WorkerService.DI;

public static class JobScheduler
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddWorkerServiceDependencies(builder.Configuration);
        
        builder.Services.AddHostedService<CheckItemsLeftOnShelfAgainstLowStockThresholdWorkerService>();
        builder.Services.AddHostedService<CheckForExpiringProductsWorkerService>();
        
        return builder.Build();
    }

    public static WebApplication AddJobScheduler(this WebApplication app)
    {
        return app;
    }
}