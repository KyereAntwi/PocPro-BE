namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IPromoCodesServices
{
    Task<(DateTimeOffset expiryDate, decimal discountPercentage)?> GetPromoCodeDetailsAsync(
        string promoCode,
        CancellationToken cancellationToken = default);
    
    Task <bool> UsePromoCodeAsync(
        string promoCode,
        string userId,
        CancellationToken cancellationToken = default);
}