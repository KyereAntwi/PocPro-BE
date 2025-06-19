namespace DevSync.PocPro.Shops.StocksModule.Features.Suppliers.V1.GetAllSuppliers;

public class GetAllSuppliersEndpoint(IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<GetAllSuppliersRequest, BaseResponse<PagedResponse<GetSupplierResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/suppliers");
    }

    public override async Task HandleAsync(GetAllSuppliersRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_SUPPLIERS, userId);

        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var query = shopDbContext.Suppliers.Include(s => s.Contacts).AsNoTracking().AsSplitQuery();

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            query = query.Where(s => 
                s.Title.ToLower().Contains(req.Keyword.ToLower()) ||
                s.Contacts.Select(c => c.Person.ToLower()).Contains(req.Keyword.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(req.SortBy))
        {
            switch (req.SortBy)
            {
                case "created-asc":
                    query = query.OrderBy(s => s.CreatedAt);
                    break;
                case "title-asc":
                    query = query.OrderBy(s => s.Title);
                    break;
                case "title-desc":
                    query = query.OrderByDescending(s => s.Title);
                    break;
                default:
                    query = query.OrderByDescending(s => s.CreatedAt);
                    break;
                    
            }
        }
        
        var totalCount = await query.CountAsync(ct);

        var pagedList = await query.Select(s => new GetSupplierResponse(
                s.Id.Value,
                s.Title,
                s.Email ?? string.Empty,
                s.Contacts.Select(c => new ContactResponse(
                    c.Id.Value,
                    c.Person,
                    c.Value,
                    c.ContactType.ToString())),
                s.CreatedAt,
                s.UpdatedAt,
                s.Status.ToString() ?? StatusType.Active.ToString()))
            .ToArrayAsync(ct);

        var response = new PagedResponse<GetSupplierResponse>(req.Page, req.PageSize, totalCount, pagedList);

        await SendOkAsync(new BaseResponse<PagedResponse<GetSupplierResponse>>("Supplers fetched successfully", true)
        {
            Data = response
        }, ct);
    }
}