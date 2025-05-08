namespace DevSync.PocPro.Shops.PrivateCustomers.DI;

public static class CustomerModuleRegistration
{
    public static IServiceCollection AddCustomerModule(this IServiceCollection services)
    {
        services.AddDbContext<CustomerModuleDbContext>(opt =>
        {
            opt.UseNpgsql("postgres");
        });

        services.AddScoped<ICustomerDbContext, CustomerModuleDbContext>();
        
        return services;
    }
}