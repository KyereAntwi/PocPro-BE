namespace DevSync.PocPro.Shops.OrdersModule.Domain.Services;

public interface IExternalServices
{
    Task<Result<ProductDetails>> GetProductDetailsAsync(Guid productId);
    Task<Result<PosDetails>> GetPosDetailsAsync(Guid posId);
}

public class ExternalServices(
    ProductService.ProductServiceClient productServiceClient, 
    PointOfSaleService.PointOfSaleServiceClient pointOfSaleServiceClient,
    ILogger<ExternalServices> logger) 
    : IExternalServices
{
    public async Task<Result<ProductDetails>> GetProductDetailsAsync(Guid productId)
    {
        try
        {
            var request = new GetProductDetailsRequest { Id = productId.ToString() };
            var response = await productServiceClient.GetProductDetailsAsync(request);
            return Result.Ok(new ProductDetails(response.Id, response.Name, response.PhotoUrl, response.Price));
        }
        catch (Exception ex)
        {
            logger.LogError("Product not found for Id {Product}: {Message}", productId, ex.Message);
            return Result.Fail("Product not found");
        }
    }

    public async Task<Result<PosDetails>> GetPosDetailsAsync(Guid posId)
    {
        try
        {
            var request = new GetPointOfSaleDetailsRequest {Id = posId.ToString() };
            var response = await pointOfSaleServiceClient.GetPointOfSaleDetailsAsync(request);
            return Result.Ok(new PosDetails(response.Id, response.Title));
        }
        catch (Exception ex)
        {
            logger.LogError("Pos not found for Id {Pos}: {Message}", posId, ex.Message);
            return Result.Fail("Pos not found");
        }
    }
}

public record ProductDetails(string Id, string Name, string PhotoUrl, double Price);

public record PosDetails(string Id, string Title);

