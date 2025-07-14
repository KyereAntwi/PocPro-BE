using DevSync.PocPro.Shops.Shared.Utils;

namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductsForWorkerService;

public class GetProductsExpiringSoonEndpoint(
    IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ApiAuthentication authentication)
    : Endpoint<GetProductsExpiringSoonRequest, IEnumerable<ProductDto>>
{
    public override void Configure()
    {
        Get("/api/v1/products/for-service-worker/get-products-expiring-soon");
        AllowAnonymous();
        Description(x => x
            .WithName("GetProductsExpiringSoon")
            .Produces<IEnumerable<ProductDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError));
    }

    public override async Task HandleAsync(GetProductsExpiringSoonRequest req, CancellationToken ct)
    {
        var apiKey = httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"].ToString();

        if (string.IsNullOrWhiteSpace(apiKey) || !apiKey.Equals(authentication.ApiKey))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(req.Pos))
        {
            await SendErrorsAsync((int)HttpStatusCode.BadRequest, ct);
            return;
        }
       
        var posIds = req.Pos
            .Split(";")
            .Select(p => PointOfSaleId.Of(Guid.Parse(p)));
        
        var response = new List<ProductDto>();

        foreach (var posId in posIds)
        {
            var stock =
                await shopDbContext
                    .Stocks
                    .Where(s => posId == s.PointOfSaleId)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstAsync(ct);

            if (stock.ExpiresAt > DateTime.UtcNow.AddMonths(1)) continue;
            
            var products = await shopDbContext.
                Products
                .Where(p => p.Id == stock.ProductId)
                .Select(p => new ProductDto(p.Id.Value, p.Name, p.PhotoUrl ?? string.Empty))
                .AsNoTracking()
                .ToArrayAsync(ct);
            
            response.AddRange(products);
        }

        await SendOkAsync(response, ct);
    }
}

public record GetProductsExpiringSoonRequest(string Pos);