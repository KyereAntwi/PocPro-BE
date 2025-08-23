namespace DevSync.PocPro.Shops.PromoCodesModule.Services;

public class PromoCodesServices 
    (IPromoCodeModuleDbContext promoCodeModuleDb): IPromoCodesServices
{
    public async Task<(DateTimeOffset expiryDate, decimal discountPercentage)?> GetPromoCodeDetailsAsync(string promoCode, CancellationToken cancellationToken = default)
    {
        var existingPromoCode = await promoCodeModuleDb
            .PromoCodes
            .Where(p => p.Code == promoCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingPromoCode is null)
        {
            return null;
        }
        
        return (existingPromoCode.ExpiryDate, existingPromoCode.DiscountPercentage);
    }

    public async Task<bool> UsePromoCodeAsync(string promoCode, string userId, CancellationToken cancellationToken = default)
    {
        var existingPromoCode = await promoCodeModuleDb
            .PromoCodes.Include(p => p.PromoCodeUsers)
            .AsSplitQuery()
            .Where(p => p.Code == promoCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingPromoCode is null)
        {
            return false;
        }

        var response = existingPromoCode.UseCode(userId);
        
        if (response.IsFailed)
        {
            return false;
        }
        
        promoCodeModuleDb.PromoCodes.Update(existingPromoCode);
        return await promoCodeModuleDb.SaveChangesAsync(cancellationToken) > 0;
    }
}