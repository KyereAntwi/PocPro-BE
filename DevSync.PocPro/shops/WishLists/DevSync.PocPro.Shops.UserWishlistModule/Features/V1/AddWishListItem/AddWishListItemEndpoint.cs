namespace DevSync.PocPro.Shops.UserWishlistModule.Features.V1.AddWishListItem;

public class AddWishListItemEndpoint(IWishListDbContext wishListDbContext, IHttpContextAccessor httpContextAccessor) : Endpoint<AddWishListItemRequest>
{
    public override void Configure()
    {
        Post("/api/v1/wishlists");
        Description(x => x
            .WithName("AddWishListItem")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized));
    }

    public override async Task HandleAsync(AddWishListItemRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        var result = WishListItem.Create(ProductId.Of(req.ProductId), userId!);
        await wishListDbContext.WishListItems.AddAsync(result, ct);
        await wishListDbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}