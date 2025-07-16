namespace DevSync.Pocpro.Shops.Notifications.DI;

public static class RegisterNotificationsDependencies
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services, IConfigurationBuilder builder)
    {
        // services
        //     .AddSignalR()
        //     .AddAzureSignalR(builder.Properties["SignalRConnectionString"] as string);
        
        return services;
    }
}