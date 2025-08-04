namespace DevSync.PocPro.Shops.Reviews.DI;

public static class RegisterReviewsModule
{
    public static IServiceCollection AddReviewsModuleDependencies(this IServiceCollection services)
    {
        services.AddDbContext<ProductReviewsModuleDbContext>();
        services.AddScoped<IProductReviewsModuleDbContext, ProductReviewsModuleDbContext>();
        services.AddScoped<IReviewsServices, ReviewsServices>();
        
        return services;
    }
}