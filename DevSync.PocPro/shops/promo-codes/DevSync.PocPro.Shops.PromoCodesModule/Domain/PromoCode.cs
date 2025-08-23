namespace DevSync.PocPro.Shops.PromoCodesModule.Domain;

public class PromoCode : BaseEntity<Guid>
{
    private Collection<PromoCodeUser> _promoCodeUsers = [];
    public IReadOnlyCollection<PromoCodeUser> PromoCodeUsers => _promoCodeUsers;
    
    public static Result<PromoCode> Create(
        string code,
        string description,
        DateTimeOffset expiryDate,
        decimal discountPercentage,
        CodeType codeType = CodeType.SingleUsePerUser)
    {
        var promoCode = new PromoCode
        {
            Id = Guid.CreateVersion7(),
            Code = code,
            Description = description,
            ExpiryDate = expiryDate,
            DiscountPercentage = discountPercentage,
            CodeType = codeType,
            Status = StatusType.Active
        };

        return promoCode;
    }
    
    public Result UseCode(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Fail("User ID cannot be empty");
        }

        if (ExpiryDate < DateTimeOffset.UtcNow)
        {
            return Result.Fail("Promo code has expired");
        }

        switch (CodeType)
        {
            case CodeType.SingleUsePerUser when _promoCodeUsers.Count != 0:
                return Result.Fail("Promo code has already been used.");
            case CodeType.MultiUsePerUser when _promoCodeUsers.Count > 0 && _promoCodeUsers.First().UserId != userId:
                return Result.Fail("Promo code has already been used by another user.");
            case CodeType.GeneralMultiUse:
                break;
            default:
                return Result.Fail("Invalid promo code type.");
        }

        var promoCodeUser = new PromoCodeUser(Id, userId);
        _promoCodeUsers.Add(promoCodeUser);

        return Result.Ok();
    }
    
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public CodeType CodeType { get; private set; } = CodeType.SingleUsePerUser;
    public DateTimeOffset ExpiryDate { get; private set; }
    public decimal DiscountPercentage { get; private set; }
}