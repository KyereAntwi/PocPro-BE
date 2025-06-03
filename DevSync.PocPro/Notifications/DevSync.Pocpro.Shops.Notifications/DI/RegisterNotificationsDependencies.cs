namespace DevSync.Pocpro.Shops.Notifications.DI;

public static class RegisterNotificationsDependencies
{
    public static IServiceCollection RegisterNotifications(this IServiceCollection services, IConfigurationBuilder builder)
    {
        services
            .AddSignalR()
            .AddAzureSignalR((string)builder.Properties["SignalRConnectionString"]);
        
        return services;
    }
}