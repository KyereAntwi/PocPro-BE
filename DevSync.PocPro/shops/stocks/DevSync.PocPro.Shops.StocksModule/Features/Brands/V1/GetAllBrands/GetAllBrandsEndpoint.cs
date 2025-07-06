namespace DevSync.PocPro.Shops.StocksModule.Features.Brands.V1.GetAllBrands;

public class GetAllBrandsEndpoint(IShopDbContext shopDbContext) : Endpoint<GetAllBrandsRequest, BaseResponse<IEnumerable<BrandResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/brands");
        Description(x => x
            .WithName("GetAllBrands")
            .Produces<BaseResponse<IEnumerable<BrandResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError));
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(GetAllBrandsRequest req, CancellationToken ct)
    {
        var brands = shopDbContext
            .Brands
            .Select(b => new BrandResponse(b.Id.Value, b.Title, b.Description, b.ImageUrl))
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            brands = brands.Where(x => x.Title.ToLower().Contains(req.SearchText.ToLower()));
        }

        await SendOkAsync(new BaseResponse<IEnumerable<BrandResponse>>("Brands retrieved successfully", true)
        {
            Data = await brands.ToArrayAsync(ct)
        }, ct);
    }
}