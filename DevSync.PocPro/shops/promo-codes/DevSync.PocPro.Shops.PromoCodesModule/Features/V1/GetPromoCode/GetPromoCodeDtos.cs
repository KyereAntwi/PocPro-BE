namespace DevSync.PocPro.Shops.PromoCodesModule.Features.V1.GetPromoCode;

public record GetPromoCodeRequest(
    Guid Id);

public record PromoCodeResponse(
    Guid Id,
    string Code,
    string CodeType,
    decimal DiscountPercentage,
    DateTimeOffset ExpiryDate,
    string Description,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    string CreatedBy)
{
    public IEnumerable<PromoCodeUserDto> Users { get; set; } = [];
}
    
public record PromoCodeUserDto(string UserId);