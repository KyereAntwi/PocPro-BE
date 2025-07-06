using DevSync.PocPro.Shops.UserWishlistModule.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DevSync.PocPro.Shops.UserWishlistModule.DI;

public static class WishListModuleRegistration
{
    public static IServiceCollection AddWishListModule(this IServiceCollection services)
    {
        services.AddDbContext<UserWishListModuleDbContext>();
        services.AddScoped<IWishListDbContext, UserWishListModuleDbContext>();
        return services;
    }
}