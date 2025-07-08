namespace DevSync.PocPro.Shops.WorkerService.DI;

public static class JobScheduler
{
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
        
        app.UseHangfireDashboard("/hangfire", new DashboardOptions()
        {
            Authorization = [new HangfireAuthorizationFilter()]
        });
        
        return app;
    }
}