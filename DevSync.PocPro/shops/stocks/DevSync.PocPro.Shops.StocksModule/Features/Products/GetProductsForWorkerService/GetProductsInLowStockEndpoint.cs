using DevSync.PocPro.Shops.Shared.Utils;

namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductsForWorkerService;

public class GetProductsInLowStockEndpoint(
    IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ApiAuthentication authentication) 
    : Endpoint<GetProductsInLowStockRequest, IEnumerable<ProductDto>>
{
    public override void Configure()
    {
        Get("/api/v1/products/for-service-worker/get-products-low-in-stock");
        AllowAnonymous();
        Description(x => x
            .WithName("GetProductsInLowStock")
            .Produces<IEnumerable<ProductDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError));
    }

    public override async Task HandleAsync(GetProductsInLowStockRequest req, CancellationToken ct)
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
           var products = await shopDbContext
               .Products
               .Include(p => p.Stocks)
               .Where(p => p.Stocks.LongCount() > 0 && p
                   .Stocks
                   .OrderByDescending(s => s.CreatedAt)
                   .Last()
                   .QuantityLeftInStock <= p.LowThresholdValue)
               .Select(p => new ProductDto(p.Id.Value, p.Name, p.PhotoUrl ?? string.Empty, 0))
               .AsSplitQuery()
               .AsNoTracking()
               .ToArrayAsync(ct);
           
              response.AddRange(products);
       }
       
       await SendOkAsync(response, ct);
    }
}

public record GetProductsInLowStockRequest(string Pos);