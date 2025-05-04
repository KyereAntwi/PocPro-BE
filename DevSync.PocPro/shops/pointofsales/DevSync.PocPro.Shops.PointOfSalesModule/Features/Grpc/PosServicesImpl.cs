using Grpc.Core;

namespace DevSync.PocPro.Shops.PointOfSales.Features.Grpc;

public class PosServicesImpl : PointOfSaleService.PointOfSaleServiceBase
{
    private readonly IPOSDbContext _posDbContext;

    public PosServicesImpl(IPOSDbContext posDbContext)
    {
        _posDbContext = posDbContext;
    }

    public override async Task<GetPointOfSaleDetailsResponse> GetPointOfSaleDetails(GetPointOfSaleDetailsRequest request, ServerCallContext context)
    {
        var pos = await _posDbContext.PointOfSales.FindAsync(PointOfSaleId.Of(Guid.Parse(request.Id)));
        
        if (pos == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Point of Sale not found"));
        }

        return new GetPointOfSaleDetailsResponse()
        {
            Id = pos.Id.Value.ToString(),
            Title = pos.Title,
        };
    }
}