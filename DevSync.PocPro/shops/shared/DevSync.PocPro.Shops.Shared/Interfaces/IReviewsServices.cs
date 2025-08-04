using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IReviewsServices
{
    Task<float> GetProductRatingAsync(ProductId productId, CancellationToken cancellationToken = default);
}