namespace DevSync.PocPro.Shops.PromoCodesModule.Features.V1.GetPromoCode;

public class GetPromoCodeEndpoint(
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices,
    IPromoCodeModuleDbContext promoCodeModuleDbContext) 
    : Endpoint<GetPromoCodeRequest, BaseResponse<PromoCodeResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/promo-codes/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetPromoCodeRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var hasPermission = await tenantServices.UserHasRequiredPermissionAsync(
            PermissionType.MANAGE_PROMO_CODES, userId!);

        if (!hasPermission)
        {
            await SendAsync(
                new BaseResponse<PromoCodeResponse>("Permission Denied", false)
                {
                    Errors = ["You do not have required permission"]
                },
                StatusCodes.Status403Forbidden, ct);
            return;
        }
        
        var promoCode = await promoCodeModuleDbContext
            .PromoCodes
            .Include(pc => pc.PromoCodeUsers)
            .AsNoTracking()
            .AsSplitQuery()
            .Where(pc => pc.Id == req.Id)
            .Select(pc => new PromoCodeResponse(
                pc.Id,
                pc.Code,
                pc.CodeType.ToString(),
                pc.DiscountPercentage,
                pc.ExpiryDate,
                pc.Description,
                pc.CreatedAt,
                pc.UpdatedAt,
                pc.CreatedBy ?? string.Empty)
            {
                Users = pc.PromoCodeUsers.Select(u => new PromoCodeUserDto(u.UserId))
            })
            .FirstOrDefaultAsync(ct);

        if (promoCode is null)
        {
            await SendAsync(
                new BaseResponse<PromoCodeResponse>("Promo code not found", false)
                {
                    Errors = ["The requested promo code does not exist"]
                },
                StatusCodes.Status404NotFound, ct);
            return;
        }
        
        await SendAsync(
            new BaseResponse<PromoCodeResponse>("Promo code retrieved successfully", true)
            {
                Data = promoCode
            }, cancellation: ct);
    }
}