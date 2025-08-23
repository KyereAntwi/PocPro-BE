namespace DevSync.PocPro.Shops.PromoCodesModule.Features.V1.CreateAPromoCode;

public class CreateAPromoCodeEndpoint(
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices,
    IPromoCodeModuleDbContext promoCodeModuleDbContext) 
    : Endpoint<CreateAPromoCodeRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/promo-codes");
    }

    public override async Task HandleAsync(CreateAPromoCodeRequest req, CancellationToken ct)
    {
        var codeType = Enum.Parse<CodeType>(req.CodeType);
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var hasPermission = await tenantServices.UserHasRequiredPermissionAsync(
            PermissionType.MANAGE_PROMO_CODES, userId!);

        if (!hasPermission)
        {
            await SendAsync(
                new BaseResponse<Guid>("Permission Denied", false)
                {
                    Errors = ["You do not have required permission"]
                },
                StatusCodes.Status403Forbidden, ct);
            return;
        }
        
        var promoCode = PromoCode.Create(
            codeType: codeType,
            code: req.Code,
            discountPercentage: req.DiscountPercentage,
            expiryDate: req.ExpiryDate,
            description: req.Description);

        if (promoCode.IsFailed)
        {
            await SendAsync(
                new BaseResponse<Guid>("Bad Request", false)
                {
                    Errors = promoCode.Errors.Select(e => e.Message).ToList()
                },
                StatusCodes.Status403Forbidden, ct);
            return;
        }
        
        await promoCodeModuleDbContext.PromoCodes.AddAsync(promoCode.Value, ct);
        await promoCodeModuleDbContext.SaveChangesAsync(ct);
        
        await SendCreatedAtAsync<GetPromoCodeEndpoint>(
            new
            {
                Code = promoCode.Value.Id
            },
            new BaseResponse<Guid>("Promo code created successfully", true)
            {
                Data = promoCode.Value.Id
            }, cancellation: ct);
    }
}

public class CreateAPromoCodeRequestValidator : Validator<CreateAPromoCodeRequest>
{
    public CreateAPromoCodeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Promo code cannot be empty.")
            .MaximumLength(50)
            .WithMessage("Promo code cannot exceed 50 characters.");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(1, 100)
            .WithMessage("Discount percentage must be between 1 and 100.");
        
        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiration date must be in the future.");
        
        RuleFor(x => x.CodeType)
            .Must(x => Enum.TryParse<CodeType>(x, true, out _))
            .WithMessage("Invalid code type specified.");
    }
}