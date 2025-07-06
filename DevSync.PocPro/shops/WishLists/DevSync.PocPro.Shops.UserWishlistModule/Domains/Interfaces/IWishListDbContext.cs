namespace DevSync.PocPro.Shops.UserWishlistModule.Domains.Interfaces;

public interface IWishListDbContext
{
    DbSet<WishListItem> WishListItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}