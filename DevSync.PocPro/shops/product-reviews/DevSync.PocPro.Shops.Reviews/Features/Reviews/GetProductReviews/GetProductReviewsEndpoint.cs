namespace DevSync.PocPro.Shops.Reviews.Features.Reviews.GetProductReviews;

public class GetProductReviewsEndpoint(IProductReviewsModuleDbContext dbContext) 
    : Endpoint<GetProductReviewsRequest, BaseResponse<PagedResponse<ReviewDto>>>
{
    public override void Configure()
    {
        Get("/api/v1/products/{ProductId}/reviews");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProductReviewsRequest req, CancellationToken ct)
    {
        var query = dbContext
            .ProductReviews
            .Where(r => r.ProductId == ProductId.Of(req.ProductId))
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            query = query.Where(r => r.Message.ToLower().Contains(req.SearchText.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(req.CreatedBy))
        {
            query = query.Where(r => r.CreatedBy! == req.CreatedBy);
        }

        var totalCount = await query.LongCountAsync(ct);
        var pagedList = await query
            .Select(r => new ReviewDto(
                r.Message,
                r.CreatedBy ?? string.Empty,
                r.CreatedAt))
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToArrayAsync(ct);

        var response = new PagedResponse<ReviewDto>(
            req.Page,
            req.PageSize,
            totalCount,
            pagedList);

        await SendOkAsync(new BaseResponse<PagedResponse<ReviewDto>>("Reviews fetched successfully", true)
        {
            Data = response
        }, ct);
    }
}