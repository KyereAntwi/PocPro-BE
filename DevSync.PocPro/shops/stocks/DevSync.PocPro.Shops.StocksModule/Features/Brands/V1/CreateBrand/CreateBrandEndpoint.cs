namespace DevSync.PocPro.Shops.StocksModule.Features.Brands.V1.CreateBrand;

public class CreateBrandEndpoint(IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<CreateBrandRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/brands");
        Description(x => x
            .WithName("CreateBrand")
            .Produces<BaseResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError));
    }

    public override async Task HandleAsync(CreateBrandRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_BRANDS, userId!))
        {
            await SendAsync(new BaseResponse<Guid>("You do not have permission to create a brand." , false)
            {
                Errors = new List<string> { "Forbidden" }
            }, (int)HttpStatusCode.Forbidden , ct);
            return;
        }
        
        var existingBrand = await shopDbContext.Brands.FirstOrDefaultAsync(b => b.Title.ToLower() == req.Title.ToLower(), ct);

        if (existingBrand != null)
        {
            await SendAsync(new BaseResponse<Guid>("Operation failed" , false)
            {
                Errors = new List<string> { $"A brand with title {req.Title} already exists." }
            }, (int)HttpStatusCode.BadRequest , ct);
            return;
        }
        
        var newBrand = Brand.Create(req.Title, req.Description, req.ImageUrl);
        await shopDbContext.Brands.AddAsync(newBrand, ct);
        await shopDbContext.SaveChangesAsync(ct);
        
        await SendCreatedAtAsync<GetBrandEndpoint>(new { Id = newBrand.Id }, new BaseResponse<Guid>("Brand created successfully", true)
        {
            Data = newBrand.Id.Value
        }, cancellation: ct);
    }
}

public class CreateBrandRequestValidator : Validator<CreateBrandRequest>
{
    public CreateBrandRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull();
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("{PropertyName} must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
        
        RuleFor(x => x.ImageUrl)
            .Must(BeAValidUrl).WithMessage("{PropertyName} must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true; // Allow null or empty
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && 
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}