namespace DevSync.PocPro.Shops.Reviews.Services;

public class ReviewsServices(IProductReviewsModuleDbContext dbContext) 
    : IReviewsServices
{
    public async Task<float> GetProductRatingAsync(ProductId productId, CancellationToken cancellationToken = default)
    {
        var query = await dbContext
            .Ratings
            .Where(x => x.ProductId == productId)
            .ToListAsync(cancellationToken);
        
        var totalSumOfRatings = query.Sum(r => r.StarRating);
        return totalSumOfRatings / query.Count;
    }
}