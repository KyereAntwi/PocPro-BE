namespace DevSync.PocPro.Shops.UserWishlistModule.Domains;

public class WishListItem : BaseEntity<WishListItemId>
{
    public static WishListItem Create(ProductId productId, PointOfSaleId pointOfSaleId)
    {
        return new WishListItem
        {
            Id = WishListItemId.Of(Guid.CreateVersion7()),
            ProductId = productId,
            PointOfSaleId = pointOfSaleId
        };
    }

    public ProductId ProductId { get; private set; }
    public PointOfSaleId PointOfSaleId { get; private set; }
}