namespace DevSync.PocPro.Shops.PromoCodesModule.Features.V1.GetAllPromoCodes;

public class GetAllPromoCodesEndpoint(
    IPromoCodeModuleDbContext dbContext) 
    : Endpoint<GetAllPromoCodesRequest, BaseResponse<PagedResponse<PromoCodeResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/promo-codes");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllPromoCodesRequest req, CancellationToken ct)
    {
        var query = dbContext
            .PromoCodes
            .Include(p => p.PromoCodeUsers)
            .AsNoTracking()
            .AsSplitQuery();

        query = FilterCodes(req, query);

        var totalCount = await query.CountAsync(ct);
        var promoCodes = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((req.PageNumber - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(p => new PromoCodeResponse(
                p.Id,
                p.Code,
                p.CodeType.ToString(),
                p.DiscountPercentage,
                p.ExpiryDate,
                p.Description,
                p.CreatedAt,
                p.UpdatedAt,
                p.CreatedBy ?? string.Empty))
            .ToListAsync(ct);
        
        var response = new PagedResponse<PromoCodeResponse>(
            req.PageNumber,
            req.PageSize,
            totalCount,
            promoCodes);
        
        await SendOkAsync(new BaseResponse<PagedResponse<PromoCodeResponse>>("Promo codes retrieved successfully", true)
        {
            Data = response
        }, ct);
    }

    private static IQueryable<PromoCode> FilterCodes(GetAllPromoCodesRequest req, IQueryable<PromoCode> query)
    {
        if (!string.IsNullOrWhiteSpace(req.Code))
        {
            query = query.Where(p => p.Code == req.Code);
        }
        
        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            var searchText = req.SearchText.ToLower().Trim();
            query = query.Where(p => p.Description.ToLower().Contains(searchText));
        }
        
        if (req.MinDiscountPercentage > 0)
        {
            query = query.Where(p => p.DiscountPercentage >= req.MinDiscountPercentage);
        }

        if (req.MaxDiscountPercentage < 100)
        {
            query = query.Where(p => p.DiscountPercentage <= req.MaxDiscountPercentage);
        }
        
        if (!req.IncludeExpired)
        {
            query = query.Where(p => p.ExpiryDate >= DateTimeOffset.UtcNow);
        }
        
        if (!string.IsNullOrWhiteSpace(req.UsedByUser))
        {
            query = query.Where(p => p.PromoCodeUsers.Any(u => u.UserId == req.UsedByUser));
        }

        return query;
    }
}