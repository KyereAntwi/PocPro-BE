namespace DevSync.PocPro.Shops.PromoCodesModule.Domain;

public class PromoCodeUser : BaseEntity<Guid>
{
    private PromoCodeUser() {}

    internal PromoCodeUser(Guid promoCodeId, string userId)
    {
        PromoCodeId = promoCodeId;
        UserId = userId;
    }

    public Guid PromoCodeId { get; private set; }
    public string UserId { get; set; } = string.Empty;
}