using FluentValidation;

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
        var userId = httpContextAccessor
            .HttpContext!
            .User
            .Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var result = WishListItem.Create(ProductId.Of(req.ProductId), PointOfSaleId.Of(req.ProductId));
        await wishListDbContext.WishListItems.AddAsync(result, ct);
        await wishListDbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}

public class AddWishListItemsRequestValidator : Validator<AddWishListItemRequest>
{
    public AddWishListItemsRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product id is required")
            .NotNull();

        RuleFor(x => x.PosId)
            .NotEmpty().WithMessage("Pos id is required")
            .NotNull();
    }
}