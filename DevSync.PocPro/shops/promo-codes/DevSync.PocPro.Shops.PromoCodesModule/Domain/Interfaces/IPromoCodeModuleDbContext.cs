namespace DevSync.PocPro.Shops.PromoCodesModule.Domain.Interfaces;

public interface IPromoCodeModuleDbContext
{
    DbSet<PromoCode> PromoCodes { get; }
    DbSet<PromoCodeUser> PromoCodeUsers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}