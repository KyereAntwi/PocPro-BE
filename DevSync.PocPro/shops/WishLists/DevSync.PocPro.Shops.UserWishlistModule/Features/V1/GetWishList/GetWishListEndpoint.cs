namespace DevSync.PocPro.Shops.UserWishlistModule.Features.V1.GetWishList;

public class GetWishListEndpoint(
    IWishListDbContext wishListDbContext, IProductServices product, IHttpContextAccessor httpContextAccessor) 
    : Endpoint<GetWishListRequest, BaseResponse<PagedResponse<GetWishListItemResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/wishlist");
        Description(x => x
            .WithName("GetWishList")
            .Produces<BaseResponse<PagedResponse<GetWishListItemResponse>>>()
            .Produces<BaseResponse<string>>(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized));
    }

    public override async Task HandleAsync(GetWishListRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

        var list = await wishListDbContext.WishListItems
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);

        List<GetWishListItemResponse> items = [];
        
        var productTasks = list
            .Select(item => product.GetProductByIdAsync(item.ProductId.Value, ct))
            .ToList();
        
        var productDtos = await Task.WhenAll(productTasks);
        
        for (var i = 0; i < list.Count; i++)
        {
            var productDto = productDtos[i];
            if (productDto != null)
            {
                items.Add(new GetWishListItemResponse(productDto, list[i].CreatedAt));
            }
        }

        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            items = items.Where(x => x.Product.Name.Contains(req.SearchText)).ToList();
        }
        
        var pagedItems = items
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToList();
        
        var totalCount = items.Count;
        
        var response = new PagedResponse<GetWishListItemResponse>(
            req.Page,
            req.PageSize,
            totalCount,
            pagedItems);

        await SendOkAsync(
            new BaseResponse<PagedResponse<GetWishListItemResponse>>("WishList retrieved successfully", true)
            {
                Data = response
            }, ct);
    }
}