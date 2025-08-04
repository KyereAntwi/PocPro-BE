namespace DevSync.PocPro.Shops.Reviews.Features.Ratings.RateProduct;

public class RateProductEndpoint(
    IProductReviewsModuleDbContext dbContext, 
    IHttpContextAccessor httpContextAccessor, 
    IProductServices productServices, 
    RabbitMqConnectionFactory rabbitMqConnection,
    ILogger<RateProductEndpoint> logger,
    ITenantServices tenantServices,
    IReviewsServices reviewsServices)
    : Endpoint<RateProductRequest>
{
    public override void Configure()
    {
        Post("/api/v1/products/{ProductId}/ratings");
    }

    public override async Task HandleAsync(RateProductRequest req, CancellationToken ct)
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
                    Errors = ["User has already submitted a rating for product"]
                }, (int)HttpStatusCode.BadRequest, ct);
            
            return;
        }

        await dbContext.Ratings.AddAsync(new Rating
        {
            ProductId = ProductId.Of(req.ProductId),
            StarRating = req.Rating
        }, ct);

        await dbContext.SaveChangesAsync(ct);

        try
        {
            var tenantTask = tenantServices.GetTenantByUserIdAsync(userId!);
            var ratingsTask = reviewsServices.GetProductRatingAsync(ProductId.Of(req.ProductId), ct);
            
            await Task.WhenAll(
                tenantTask,
                ratingsTask
            );
            
            var tenant = await tenantTask;
            var ratings = await ratingsTask;
            
            var sendRatedProductEvent = new SendProductRatedEvent
            {
                ProductId = req.ProductId,
                Identifier = tenant!.UniqueIdentifier,
                Ratings = ratings
            };

            var publisher = new EventPublisher(rabbitMqConnection);
            await publisher.PublishAsync(sendRatedProductEvent, "shop_exchange", "update_product_rated", ct);
        }
        catch (Exception e)
        {
            logger.LogError("Error publishing SendProductRatedEvent: {Message}", e.Message);
        }

        await SendNoContentAsync(ct);
    }
}

public class RateProductRequestValidator : Validator<RateProductRequest>
{
    public RateProductRequestValidator()
    {
        RuleFor(x => x.Rating)
            .GreaterThanOrEqualTo(0).WithMessage("Rating must be above 0")
            .LessThanOrEqualTo(5).WithMessage("Rating must be less than or equal to 5")
            .NotNull();
    }
}