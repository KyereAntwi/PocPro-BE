using Grpc.Core;

namespace DevSync.PocPro.Shops.StocksModule.Features.Grpc;

public class ProductServicesImpl : ProductService.ProductServiceBase
{
    private readonly IShopDbContext _dbContext;

    public ProductServicesImpl(IShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GetProductDetailsResponse> GetProductDetails(GetProductDetailsRequest request, ServerCallContext context)
    {
        var product = await _dbContext
            .Products
            .Include(p => p.Stocks)
            .FirstOrDefaultAsync(p => p.Id == ProductId.Of(Guid.Parse(request.Id)));
        
        if (product == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
        }

        var response = new GetProductDetailsResponse()
        {
            Id = product.Id.Value.ToString(),
            Name = product.Name,
            PhotoUrl = product.PhotoUrl,
            Price = (double)product.Stocks.Last().SellingPerPrice
        };
        
        return response;
    }
}