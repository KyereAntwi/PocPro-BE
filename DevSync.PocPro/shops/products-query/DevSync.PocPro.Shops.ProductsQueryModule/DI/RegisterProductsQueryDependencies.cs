namespace DevSync.PocPro.Shops.ProductsQueryModule.DI;

public static class RegisterProductsQueryDependencies
{
    public static IServiceCollection AddProductQueryDependencies(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(options =>
        {
            var connectionString = configuration.GetConnectionString("ProductsQueryDBConnection");
            if (connectionString != null)
                options.Connection(connectionString);
            else
                throw new Exception("No connection string found for ProductsQueryDBConnection");
            
            options.Policies.AllDocumentsAreMultiTenanted();
            options.Schema.For<Product>().MultiTenanted();
        });

        services.AddScoped<ITenantProvider, HeaderTenantProvider>();
        
        return services;
    }
}