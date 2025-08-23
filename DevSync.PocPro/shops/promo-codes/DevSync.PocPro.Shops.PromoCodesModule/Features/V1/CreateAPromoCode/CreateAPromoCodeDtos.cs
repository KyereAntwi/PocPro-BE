namespace DevSync.PocPro.Shops.PromoCodesModule.Features.V1.CreateAPromoCode;

public record CreateAPromoCodeRequest(
    string Code,
    string Description,
    DateTimeOffset ExpiryDate,
    decimal DiscountPercentage,
    string CodeType);