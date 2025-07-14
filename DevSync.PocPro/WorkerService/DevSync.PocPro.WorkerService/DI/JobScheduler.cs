using DevSync.PocPro.WorkerService.Jobs;

namespace DevSync.PocPro.WorkerService.DI;

public static class JobScheduler
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddWorkerServiceDependencies(builder.Configuration);
        return builder.Build();
    }

    public static WebApplication AddJobScheduler(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            RecurringJob.AddOrUpdate<InventoryNotificationService>(
                "low-stock-threshold-check",
                service => service.CheckItemsLeftOnShelfAgainstLowStockThresholdAsync(),
                Cron.Hourly);
        });
        
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            RecurringJob.AddOrUpdate<InventoryNotificationService>(
                "expiring-products-check",
                service => service.CheckForExpiringProductsAsync(),
                Cron.Daily);
        });
        
        app.UseHangfireDashboard("/hangfire", new DashboardOptions());
        
        return app;
    }
}