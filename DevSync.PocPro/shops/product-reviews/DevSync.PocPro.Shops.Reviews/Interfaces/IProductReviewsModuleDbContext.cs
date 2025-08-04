namespace DevSync.PocPro.Shops.Reviews.Interfaces;

public interface IProductReviewsModuleDbContext
{
    DbSet<Rating> Ratings { get; }
    DbSet<ProductReview> ProductReviews { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}