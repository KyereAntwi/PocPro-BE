using Polly;

namespace DevSync.PocPro.Shops.OrdersModule.DI;

public static class RegisterOrderDependencies
{
    public static IServiceCollection RegisterOrderModule(this IServiceCollection services)
    {
        services.AddDbContext<OrdersModuleDbContext>(opt =>
        {
            opt.UseNpgsql("postgres");
        });

        services.AddScoped<IOrderModuleDbContext, OrdersModuleDbContext>();
        services.AddTransient<IExternalServices, ExternalServices>();
        
        services.AddGrpcClient<PointOfSaleService.PointOfSaleServiceClient>(options =>
        {
            options.Address = new Uri("https://localhost:7001");
        }).AddPolicyHandler(_ => Policy<HttpResponseMessage>
            .Handle<Grpc.Core.RpcException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
        
        services.AddGrpcClient<ProductService.ProductServiceClient>(options =>
        {
            options.Address = new Uri("https://localhost:7001");
        }).AddPolicyHandler(_ => Policy<HttpResponseMessage>
            .Handle<Grpc.Core.RpcException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
        
        return services;
    }
}