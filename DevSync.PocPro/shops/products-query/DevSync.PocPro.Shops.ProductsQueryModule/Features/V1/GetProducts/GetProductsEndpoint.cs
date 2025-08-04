namespace DevSync.PocPro.Shops.ProductsQueryModule.Features.V1.GetProducts;

public class GetProductsEndpoint(
    IDocumentStore documentStore,
    ITenantProvider provider)
    : Endpoint<GetProductsRequest, BaseResponse<PagedResponse<GetProductsResponseItem>>>
{
    public override void Configure()
    {
        Get("/api/v1/online/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProductsRequest req, CancellationToken ct)
    {
        var tenantId = provider.GetTenantId();

        if (tenantId.IsFailed)
        {
            await SendErrorsAsync((int)HttpStatusCode.BadRequest, ct);
        }
        
        await using var session = documentStore.LightweightSession(tenantId.Value);
        
        IQueryable<Product> query = session.Query<Product>();

        query = FilterProducts(req, query);
        
        if (string.IsNullOrWhiteSpace(req.Pos) && !req.IsOnline)
        {
            query = query.DistinctBy(q => q.Identifier); // ensure products are distinct
        }

        int totalCount = 0;

        try
        {
            totalCount = query.Count();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        var pagedList = await query
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(p => new GetProductsResponseItem(
                p.Identifier,
                p.Name,
                p.CurrentPrice,
                p.QuantityLeft,
                p.PhotoUrl ?? string.Empty,
                p.Category!.Id,
                p.Ratings,
                p.PosId))
            .ToListAsync(ct);

        var response = new PagedResponse<GetProductsResponseItem>(
            req.Page,
            req.PageSize,
            totalCount,
            pagedList);

        await SendOkAsync(
            new BaseResponse<PagedResponse<GetProductsResponseItem>>("Products fetched successfully", true)
            {
                Data = response
            }, ct);
    }

    private static IQueryable<Product> FilterProducts(GetProductsRequest req, IQueryable<Product> query)
    {
        if (!string.IsNullOrWhiteSpace(req.Brand))
        {
            query = query.Where(p => p.Brand!.Id == Guid.Parse(req.Brand));
        }
    
        if (!string.IsNullOrWhiteSpace(req.Category))
        {
            query = query.Where(p => p.Category!.Id == Guid.Parse(req.Category));
        }
    
        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            var keyword = req.SearchText.ToLower();
            query = query.
                Where(p => 
                    p.Name.ToLower().Contains(keyword) || 
                    p.Keywords.Any(k => k.ToLower().Contains(keyword)) ||
                    p.Category!.Title.ToLower().Contains(keyword) || 
                    p.Brand!.Title.ToLower().Contains(keyword));
        }
    
        if (req.MinPrice > 0)
        {
            query = query.Where(p => p.CurrentPrice >= req.MinPrice);
        }
    
        if (req.MinPrice > 0)
        {
            query = query.Where(p => p.CurrentPrice <= req.MaxPrice);
        }
    
        if (req.IsFeatured)
        {
            query = query.Where(p => p.IsFeatured);
        }
    
        if (req.IsOnline)
        {
            query = query.Where(p => p.OnlineEnabled);       
        }
    
        if (!string.IsNullOrWhiteSpace(req.Pos))
        {
            query = query.Where(p => p.PosId == Guid.Parse(req.Pos));       
        }

        if (!string.IsNullOrWhiteSpace(req.BarcodeNumber))
        {
            query = query.Where(p => p.BarcodeNumber == req.BarcodeNumber);
        }

        if (req.FromRatings > 0)
        {
            query = query.Where(p => p.Ratings >= req.FromRatings);
        }

        if (req.ToRatings > 0)
        {
            query = query.Where(p => p.Ratings <= req.ToRatings);
        }
    
        return query;
    }
}