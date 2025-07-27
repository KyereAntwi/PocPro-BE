namespace DevSync.PocPro.Shops.UserWishlistModule.Features.V1.AddWishListItem;

public record AddWishListItemRequest(
    Guid ProductId,
    Guid PosId);