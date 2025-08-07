namespace DevSync.PocPro.Shops.ProductsQueryModule.Features.V1.GetMostPerformingProducts;

public class GetMostPerformingProductsEndpoint(
    IDocumentStore documentStore,
    ITenantProvider provider) 
    : Endpoint<GetMostPerformingProductsRequest, BaseResponse<IEnumerable<GetProductsResponseItem>>>
{
    public override void Configure()
    {
        Get("/api/v1/products/most-performing");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetMostPerformingProductsRequest req, CancellationToken ct)
    {
        var tenantId = provider.GetTenantId();

        if (tenantId.IsFailed)
        {
            await SendErrorsAsync((int)HttpStatusCode.BadRequest, ct);
        }
        
        await using var session = documentStore.LightweightSession(tenantId.Value);
        
        IQueryable<Product> query = session.Query<Product>();
        
        query = query
            .OrderByDescending(q => q.PurchaseCount)
            .ThenByDescending(q => q.Ratings)
            .Take(req.Size);

        if (req.ForOnlineSite)
        {
            query = query.Where(p => p.OnlineEnabled);
        }
        
        await SendOkAsync(
            new BaseResponse<IEnumerable<GetProductsResponseItem>>("Products retrieved successfully", true)
            {
                Data = await query
                    .Select(p => new GetProductsResponseItem(
                        p.Identifier, 
                        p.Name, 
                        p.CurrentPrice, 
                        p.QuantityLeft, 
                        p.PhotoUrl ?? string.Empty,
                        p.Category!.Id,
                        p.Ratings,
                        p.PosId))
                    .ToListAsync(ct)
            }, ct);
    }
}

public record GetMostPerformingProductsRequest(int Size = 5, bool ForOnlineSite = true);