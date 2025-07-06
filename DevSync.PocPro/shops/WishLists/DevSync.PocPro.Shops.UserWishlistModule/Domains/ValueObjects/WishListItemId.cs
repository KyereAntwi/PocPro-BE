namespace DevSync.PocPro.Shops.UserWishlistModule.Domains.ValueObjects;

public record WishListItemId
{
    public Guid Value { get; } = Guid.Empty;
    
    private WishListItemId(Guid value) => Value = value;

    public static WishListItemId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for WishListItemId");
        }
        
        return new WishListItemId(value);
    }
}