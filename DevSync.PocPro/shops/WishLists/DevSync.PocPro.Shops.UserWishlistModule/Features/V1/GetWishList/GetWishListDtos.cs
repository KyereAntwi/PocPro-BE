namespace DevSync.PocPro.Shops.UserWishlistModule.Features.V1.GetWishList;

public record GetWishListRequest(
    string SearchText = "",
    int Page = 1,
    int PageSize = 10
);

public record GetWishListItemResponse(
    ProductDto Product
);