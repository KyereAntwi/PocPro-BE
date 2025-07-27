namespace DevSync.PocPro.Shops.UserWishlistModule.Features.V1.RemoveWishListItem;

public class RemoveWishListItemEndpoint(IWishListDbContext wishListDbContext) : Endpoint<RemoveWishListItemRequest>
{
    public override void Configure()
    {
        Delete("api/v1/wishlists/{Id}");
        Description(x => x
            .WithName("RemoveWishListItem")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized));
    }

    public override async Task HandleAsync(RemoveWishListItemRequest req, CancellationToken ct)
    {
        var item = await wishListDbContext
            .WishListItems
            .Where(item => item.ProductId == ProductId.Of(req.Id))
            .FirstOrDefaultAsync(ct);

        if (item is null)
        {
            await SendAsync(new BaseResponse<string>("Item not found", false)
            {
                Errors = [$"Item with Id {req.Id} not found in the wishlist"]
            }, (int) HttpStatusCode.NotFound, ct);
            return;
        }

        wishListDbContext.WishListItems.Remove(item!);
        await wishListDbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}