namespace DevSync.PocPro.Shops.UserWishlistModule.Domains;

public class WishListItem : BaseEntity<WishListItemId>
{
    public static WishListItem Create(ProductId productId, string userId)
    {
        return new WishListItem
        {
            Id = WishListItemId.Of(Guid.CreateVersion7()),
            ProductId = productId,
            UserId = userId
        };
    }

    public string UserId { get; private set; }
    public ProductId ProductId { get; private set; }
}