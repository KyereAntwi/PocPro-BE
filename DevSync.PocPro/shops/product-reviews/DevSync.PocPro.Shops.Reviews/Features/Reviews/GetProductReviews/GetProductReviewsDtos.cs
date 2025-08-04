namespace DevSync.PocPro.Shops.Reviews.Features.Reviews.GetProductReviews;

public record GetProductReviewsRequest(
    [FromRoute] Guid ProductId,
    string SearchText = "",
    string CreatedBy = "",
    int Page = 1,
    int PageSize = 10);
    
    public record ReviewDto(
        string Message,
        string CreatedBy,
        DateTimeOffset? CreatedAt);