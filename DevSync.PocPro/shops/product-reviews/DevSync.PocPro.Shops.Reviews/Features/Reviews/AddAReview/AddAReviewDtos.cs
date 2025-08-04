namespace DevSync.PocPro.Shops.Reviews.Features.Reviews.AddAReview;

public record AddAReviewRequest(
    Guid ProductId,
    string Message);