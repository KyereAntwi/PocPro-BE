namespace DevSync.PocPro.Shops.Reviews.Features.Reviews.DeleteReview;

public class DeleteReviewEndpoint(
    IProductReviewsModuleDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) 
    : Endpoint<DeleteReviewRequest>
{
    public override void Configure()
    {
        Delete("/api/v1/products/{ProductId}/reviews/{Id}");
    }

    public override async Task HandleAsync(DeleteReviewRequest req, CancellationToken ct)
    {
        var existingReview = await dbContext
            .ProductReviews
            .Where(r => r.ProductId == ProductId.Of(req.ProductId) && r.Id == req.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
        
        if (existingReview is null)
        {
            await SendAsync(
                new BaseResponse<string>("Notfound", false)
                {
                    Errors = ["Review not found"]
                }, (int)HttpStatusCode.BadRequest, ct);
        }
        
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!existingReview!.CreatedBy!.Equals(userId))
        {
            await SendAsync(
                new BaseResponse<string>("Forbidden", false)
                {
                    Errors = ["You don't have permission to delete this review"]
                }, (int)HttpStatusCode.Forbidden, ct);
        }

        dbContext.ProductReviews.Remove(existingReview);
        await dbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}

public record DeleteReviewRequest(Guid ProductId, Guid Id);