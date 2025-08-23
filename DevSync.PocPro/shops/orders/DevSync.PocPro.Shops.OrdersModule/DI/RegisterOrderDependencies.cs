namespace DevSync.PocPro.Shops.OrdersModule.DI;

public static class RegisterOrderDependencies
{
    public static IServiceCollection RegisterOrderModule(this IServiceCollection services, IConfigurationBuilder configuration)
    {
        services.AddDbContext<OrdersModuleDbContext>();

        services.AddScoped<IOrderModuleDbContext, OrdersModuleDbContext>();
        services.AddScoped<IOrdersServices, OrderExternalServices>();
        services.AddScoped<IPaymentServices, PaymentService>();
        
        return services;
    }
}