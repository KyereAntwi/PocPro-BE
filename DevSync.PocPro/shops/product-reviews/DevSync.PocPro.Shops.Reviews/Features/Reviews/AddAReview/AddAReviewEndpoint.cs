namespace DevSync.PocPro.Shops.Reviews.Features.Reviews.AddAReview;

public class AddAReviewEndpoint(
    IProductReviewsModuleDbContext dbContext, IHttpContextAccessor httpContextAccessor, IProductServices productServices) 
    : Endpoint<AddAReviewRequest, BaseResponse<string>>
{
    public override void Configure()
    {
        Post("/api/v1/products/{ProductId}/reviews");
    }

    public override async Task HandleAsync(AddAReviewRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var existingProduct = await productServices.GetProductByIdAsync(req.ProductId, Guid.Empty, ct);

        if (existingProduct is null)
        {
            await SendAsync(
                new BaseResponse<string>("Not found", false)
                {
                    Errors = ["Product with provided Id was not found"]
                }, (int)HttpStatusCode.NotFound, ct);
        }

        var existingReviewByUser = await dbContext
            .ProductReviews
            .Where(r => r.ProductId == ProductId.Of(req.ProductId) && r.CreatedBy == userId)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (existingReviewByUser is not null)
        {
            await SendAsync(
                new BaseResponse<string>("Bad request", false)
                {
                    Errors = ["User has already submitted a review for product"]
                }, (int)HttpStatusCode.BadRequest, ct);
            
            return;
        }

        await dbContext.ProductReviews.AddAsync(new ProductReview
        {
            ProductId = ProductId.Of(req.ProductId),
            Message = req.Message
        }, ct);

        await dbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}

public class AddAReviewRequestValidator : Validator<AddAReviewRequest>
{
    public AddAReviewRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required")
            .NotNull();

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("A review message is required")
            .NotNull();
    }
}